using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;


[RequireComponent(typeof(ScrollRect))]
public class ScrollSnapBehaviour : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    public bool IsLerping
    {
        get { return _lerp; }
    }
    public Vector2 Velocity
    {
        get
        {
            return _scrollRectComponent.velocity;
        }
    }
    public bool DidInit { get; private set; }
    public bool IsHorizontal { get { return _horizontal; } }
    public List<FAScrollSnapItem> Items { get { return items; } }
    public int startingPage = 0;
    public float fastSwipeThresholdTime = 0.3f;
    public int fastSwipeThresholdDistance = 50;
    public float decelerationRate = 5f;
    public float decelarationLimit = 10f;
    public class ScrollEvent : UnityEvent<Vector2> { }
    public class ScrollPercentageEvent : UnityEvent<float> { }
    public class ScrollToPageEvent : UnityEvent<int> { }
    public class ScrollStartedEvent : UnityEvent { }
    public class ScrollEndedEvent : UnityEvent<int> { }

    public class OnMouseDownEvent : UnityEvent { }
    public class OnMouseUpEvent : UnityEvent { }
    public ScrollEvent onScrolled;
    public ScrollPercentageEvent onScrolledPercentage;
    public ScrollToPageEvent onScrollToPage;
    public ScrollStartedEvent onScrollStarted;
    public ScrollEndedEvent onScrollEnded;
    public OnMouseDownEvent onMouseDown;
    public OnMouseUpEvent onMouseUp;

    private int _fastSwipeThresholdMaxLimit;
    private ScrollRect _scrollRectComponent;
    private RectTransform _scrollRectRect;
    private RectTransform _container;
    private bool _horizontal;
    private int _pageCount;
    private int _currentPage;
    private bool _lerp, _didFindNearest, _isDecelarating;
    private Vector2 _lerpTo;
    private List<Vector2> _pagePositions = new List<Vector2>();
    private List<FAScrollSnapItem> items = new List<FAScrollSnapItem>();
    private bool _dragging;
    private float _timeStamp;
    private Vector2 _startPosition;

    int padding;
    bool startedTouching = false;

    public void Setup()
    {
        _scrollRectComponent = GetComponent<ScrollRect>();
        _scrollRectRect = GetComponent<RectTransform>();

        if (_scrollRectComponent.horizontal && !_scrollRectComponent.vertical)
        {
            _horizontal = true;
        }
        else if (!_scrollRectComponent.horizontal && _scrollRectComponent.vertical)
        {
            _horizontal = false;
        }
        else
        {
            _horizontal = true;
        }

        _container = _scrollRectComponent.content;

        _pageCount = _container.childCount;



        _lerp = false;
        if (gameObject.activeInHierarchy)
            StartCoroutine(SetPagePositions());

    }
    public void LerpToPage(int aPageIndex)
    {
        if (!DidInit)
            return;
        if (_pagePositions == null)
            return;
        if (_pagePositions.Count <= 0)
            return;
        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
        _lerpTo = _pagePositions[aPageIndex];
        _lerp = true;
        _currentPage = aPageIndex;
    }
    public void SetScrollPosition(float pos)
    {
        _container.anchoredPosition = new Vector2(IsHorizontal ? pos : _container.anchoredPosition.x, IsHorizontal ? _container.anchoredPosition.y : pos);
    }
    private void Awake()
    {

        if (onMouseDown == null)
            onMouseDown = new OnMouseDownEvent();
        if (onMouseUp == null)
            onMouseUp = new OnMouseUpEvent();
        if (onScrolled == null)
            onScrolled = new ScrollEvent();
        if (onScrolledPercentage == null)
            onScrolledPercentage = new ScrollPercentageEvent();
        if (onScrollToPage == null)
            onScrollToPage = new ScrollToPageEvent();
        if (onScrollStarted == null)
            onScrollStarted = new ScrollStartedEvent();
        if (onScrollEnded == null)
            onScrollEnded = new ScrollEndedEvent();

    }
    void Start()
    {
        Setup();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !startedTouching)
        {
            startedTouching = true;
            onMouseDown?.Invoke();
        }
        else if (Input.GetMouseButtonUp(0) && startedTouching)
        {
            startedTouching = false;
            onMouseUp?.Invoke();

        }
        if (_lerp)
        {
            float decelerate = Mathf.Min(decelerationRate * Time.deltaTime, 1f);
            _container.anchoredPosition = Vector2.Lerp(_container.anchoredPosition, _lerpTo, decelerate);

            InvokeScrollEvents();
            if (Vector2.SqrMagnitude(_container.anchoredPosition - _lerpTo) < 10f)
            {
                _scrollRectComponent.velocity = Vector2.zero;
                _container.anchoredPosition = _lerpTo;

                _lerp = false;
                onScrollEnded?.Invoke(_currentPage);
            }

        }
        if (_isDecelarating)
        {
            // prevent overshooting with values greater than 1
            float velocity = _horizontal ? _scrollRectComponent.velocity.x : _scrollRectComponent.velocity.y;

            InvokeScrollEvents();
            if (Mathf.Abs(velocity) < decelarationLimit)
            {
                if (!_didFindNearest)
                {
                    _isDecelarating = false;
                    _didFindNearest = true;
                    int nearestPage = GetNearestPage();
                    onScrollToPage?.Invoke(nearestPage);
                    LerpToPage(nearestPage);
                }
            }

        }
    }
    private void OnDisable()
    {
        onScrolled?.RemoveAllListeners();
    }
    private IEnumerator SetPagePositions()
    {
        yield return null;
        if (_container.childCount <= 0)
            yield break;
        float width = 0;
        float height = 0;
        if (_horizontal)
        {

            float rectWidth = _scrollRectRect.rect.width;
            HorizontalLayoutGroup hlg = _container.GetComponent<HorizontalLayoutGroup>();
            if (hlg == null)
                hlg = _container.gameObject.AddComponent<HorizontalLayoutGroup>();

            width = _scrollRectRect.rect.width;
            int padding = (int)(rectWidth - width) / 2;
            hlg.padding = new RectOffset(padding, padding, 0, 0);

            padding = hlg.padding.left;
        }
        else
        {
            float rectHeight = _scrollRectRect.rect.height;
            VerticalLayoutGroup vlg = _container.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
                vlg = _container.gameObject.AddComponent<VerticalLayoutGroup>();

            height = _scrollRectRect.rect.height;
            int padding = (int)(rectHeight - height) / 2;
            vlg.padding = new RectOffset(0, 0, padding, padding);

            padding = vlg.padding.top;
        }


        _fastSwipeThresholdMaxLimit = _horizontal ? (int)_scrollRectRect.rect.width : (int)_scrollRectRect.rect.height;

        _pagePositions.Clear();
        items.Clear();

        yield return null;
        for (int i = 0; i < _pageCount; i++)
        {
            RectTransform child = _container.GetChild(i).GetComponent<RectTransform>();

            LayoutElement layoutElement = child.GetComponent<LayoutElement>();
            if (_horizontal)
            {
                layoutElement.preferredWidth = width;
                child.sizeDelta = new Vector2(width, child.rect.height);
            }
            else
            {
                layoutElement.preferredHeight = height;
                child.sizeDelta = new Vector2(child.rect.width, height);

            }

            yield return null;

            FAScrollSnapItem item = child.GetComponent<FAScrollSnapItem>();
            if (item == null)
            {
                item = child.gameObject.AddComponent<FAScrollSnapItem>();
            }

            items.Add(item);
            if (_horizontal)
                _pagePositions.Add(new Vector2(-child.anchoredPosition.x + (child.sizeDelta.x * child.pivot.x) + padding, 0));
            else
                _pagePositions.Add(new Vector2(0, -child.anchoredPosition.y - (child.sizeDelta.y * child.pivot.y) - padding));

        }
        float difference = 0;
        //Find difference between two pages
        if (_pagePositions.Count > 1)
        {
            float a = _horizontal ? Mathf.Abs(_pagePositions[1].x) : Mathf.Abs(_pagePositions[1].y);
            float b = _horizontal ? Mathf.Abs(_pagePositions[0].x) : Mathf.Abs(_pagePositions[0].y);
            difference = Mathf.Abs(a - b);
        }
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i].hasAnimations)
            {
                FAScrollSnapItemSettings settings = new FAScrollSnapItemSettings()
                {
                    Position = _horizontal ? _pagePositions[i].x : _pagePositions[i].y,
                    Difference = difference,
                    IsHorisontal = _horizontal
                };
                items[i].Setup(settings);
                onScrolled.AddListener(items[i].OnScrollHandler);
            }
        }
        if (items.Count > 0)
        {

            SetPage(startingPage);
            items[startingPage].SetSelected();

            DidInit = true;
        }
    }

    private void SetPage(int aPageIndex)
    {
        aPageIndex = Mathf.Clamp(aPageIndex, 0, _pageCount - 1);
        if (_pagePositions == null)
            return;
        if (_pagePositions.Count <= 0)
            return;
        _container.anchoredPosition = _pagePositions[aPageIndex];
        InvokeScrollEvents();
        _currentPage = aPageIndex;
    }

    public void InvokeScrollEvents()
    {
        onScrolled?.Invoke(_container.anchoredPosition);

        if (onScrolledPercentage == null)
            return;

        float t;
        if (_pagePositions.Count <= 1)
        {
            t = 1;
        }
        else if (_horizontal)
        {
            t = Mathf.InverseLerp(_pagePositions[0].x, _pagePositions[_pagePositions.Count - 1].x, _container.anchoredPosition.x);
        }
        else
        {
            t = Mathf.InverseLerp(_pagePositions[0].y, _pagePositions[_pagePositions.Count - 1].y, _container.anchoredPosition.y);
        }
        onScrolledPercentage?.Invoke(t);
    }


    private void NextScreen()
    {
        int p = _currentPage + 1;
        if (p >= _pageCount)
            return;
        LerpToPage(p);
        onScrollStarted?.Invoke();
        onScrollToPage?.Invoke(p);
    }

    private void PreviousScreen()
    {
        int p = _currentPage - 1;
        if (p < 0)
            return;
        LerpToPage(p);
        onScrollStarted?.Invoke();
        onScrollToPage?.Invoke(p);
    }
    //------------------------------------------------------------------------
    private int GetNearestPage()
    {
        // based on distance from current position, find nearest page
        Vector2 currentPosition = _container.anchoredPosition;

        float distance = float.MaxValue;
        int nearestPage = _currentPage;

        for (int i = 0; i < _pagePositions.Count; i++)
        {
            float testDist = Vector2.SqrMagnitude(currentPosition - _pagePositions[i]);
            if (testDist < distance)
            {
                distance = testDist;
                nearestPage = i;
            }
        }

        return nearestPage;
    }
    public void OnBeginDrag(PointerEventData aEventData)
    {
        _lerp = false;
        // not dragging yet
        _didFindNearest = false;
        _dragging = false;
        onScrollStarted?.Invoke();
    }

    public void OnEndDrag(PointerEventData aEventData)
    {
        startedTouching = false;
        // how much was container's content dragged
        float difference;
        if (_horizontal)
        {
            difference = _startPosition.x - _container.anchoredPosition.x;
        }
        else
        {
            difference = -(_startPosition.y - _container.anchoredPosition.y);
        }

        // test for fast swipe - swipe that moves only +/-1 item
        if (Time.unscaledTime - _timeStamp < fastSwipeThresholdTime &&
            Mathf.Abs(difference) > fastSwipeThresholdDistance &&
            Mathf.Abs(difference) < _fastSwipeThresholdMaxLimit)
        {
            if (difference > 0)
            {
                NextScreen();
            }
            else
            {
                PreviousScreen();
            }
        }
        else
        {
            int nearestPage = GetNearestPage();
            onScrollToPage?.Invoke(nearestPage);

            LerpToPage(nearestPage);
        }

        _dragging = false;
    }

    public void OnDrag(PointerEventData aEventData)
    {
        if (!_dragging)
        {
            // dragging started
            _dragging = true;
            // save time - unscaled so pausing with Time.scale should not affect it
            _timeStamp = Time.unscaledTime;
            // save current position of cointainer
            _startPosition = _container.anchoredPosition;
        }
        else
        {
            InvokeScrollEvents();
        }
    }
}


