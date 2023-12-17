using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(HorizontalLayoutGroup))]
public class TabBar : MonoBehaviour
{
    public RectTransform selector;
    public List<RectTransform> buttons;
    public ScrollSnapBehaviour scrollSnapRect;
    public int startingPage = 2;
    [Range(1, 2)]
    public float selectedBtnFlexibleWidth = 1.5f;
    public int selectedBtnPaddingTop = -30;
    public float titlePreferedHeight = 15f;

    RectTransform rectTransform;

    Vector2 selectorStartPos;
    readonly List<TabBarItem> buttonsTabItem = new List<TabBarItem>();

    FATabBarEffect effectOut;
    FATabBarEffect effectIn;

    private void Awake()
    {

        if (scrollSnapRect != null)
        {
            startingPage = scrollSnapRect.startingPage;
        }
        effectIn = new FATabBarEffect
        {
            FlexibleWidth = selectedBtnFlexibleWidth,
            PaddingTop = selectedBtnPaddingTop,
            TitlePreferedHeight = titlePreferedHeight,
            TitleFlexibleHeight = 1,
            TitleAlpha = 1
        };
        effectOut = new FATabBarEffect
        {
            FlexibleWidth = 1,
            PaddingTop = 0,
            TitlePreferedHeight = 0,
            TitleFlexibleHeight = 0,
            TitleAlpha = 0
        };
    }
    void Start()
    {
        foreach (RectTransform button in buttons)
        {
            buttonsTabItem.Add(button.GetComponent<TabBarItem>());
        }

        rectTransform = transform as RectTransform;
        StartCoroutine(Setup());
    }
    private void OnDisable()
    {

        scrollSnapRect.onScrollEnded.RemoveListener(OnScrollEnded);
        scrollSnapRect.onScrolledPercentage.RemoveListener(OnScrolled);
    }
    IEnumerator Setup()
    {
        ResetTabItems(startingPage);
        yield return null;
        selectorStartPos = buttons[0].anchoredPosition;
        int random = Random.Range(0, buttons.Count);
        while (random == startingPage)
        {
            random = Random.Range(0, buttons.Count);
            yield return null;
        }
        yield return null;
        selector.anchorMin = buttons[startingPage].anchorMin;
        selector.anchorMax = buttons[startingPage].anchorMax;
        selector.pivot = buttons[startingPage].pivot;
        selector.sizeDelta = new Vector2(buttons[startingPage].sizeDelta.x, rectTransform.sizeDelta.y);
        selector.anchoredPosition = new Vector2(SelectorPos(startingPage), selectorStartPos.y);
        scrollSnapRect.onScrolledPercentage.AddListener(OnScrolled);
        scrollSnapRect.onScrollEnded.AddListener(OnScrollEnded);
    }

    void OnScrollEnded(int page)
    {
        ResetTabItems(page);

    }
    int lastPageA, lastPageB;
    bool hasSetLastPageOnce = false;
    void OnScrolled(float percentageScrolled)
    {
        float percentMltp = percentageScrolled * ((float)buttons.Count - 1) + 1;
        int a, b;
        if (percentMltp >= buttonsTabItem.Count)
        {
            a = buttonsTabItem.Count - 2;
            b = buttonsTabItem.Count - 1;
        }
        else if (percentMltp <= 1)
        {

            a = 0;
            b = 1;
        }
        else
        {
            a = Mathf.Approximately(0, percentMltp % 1) ? (int)percentMltp - 2 : (int)Mathf.Floor(percentMltp - 1);
            b = Mathf.Approximately(0, percentMltp % 1) ? (int)percentMltp - 1 : (int)Mathf.Ceil(percentMltp - 1);

        }
        if (hasSetLastPageOnce)
        {
            if (lastPageA != a)
            {

                ResetTabItem(lastPageA);
            }
            if (lastPageB != b)
            {

                ResetTabItem(lastPageB);
            }
        }
        lastPageA = a;
        lastPageB = b;
        hasSetLastPageOnce = true;
        float t = Mathf.Approximately(b, percentMltp - 1) ? 1  : percentMltp % 1;
        float selectorAnchor = Mathf.Lerp(SelectorPos(a), SelectorPos(b), t);
        int paddingOut = (int)Mathf.Lerp(selectedBtnPaddingTop, 0, t);
        int paddingIn = (int)Mathf.Lerp(0, selectedBtnPaddingTop, t);
        float titlePreferedOut = Mathf.Lerp(titlePreferedHeight, 0, t);
        float titlePreferedIn = Mathf.Lerp(0, titlePreferedHeight, t);
        float layoutFlexibleWidthOut = Mathf.Lerp(selectedBtnFlexibleWidth, 1, t);
        float layoutFlexibleWidthIn = Mathf.Lerp(1, selectedBtnFlexibleWidth, t);

        FATabBarEffect effectOut = new FATabBarEffect
        {
            FlexibleWidth = layoutFlexibleWidthOut,
            PaddingTop = paddingOut,
            TitlePreferedHeight = titlePreferedOut,
            TitleFlexibleHeight = 1 - t,
            TitleAlpha = 1 - t
        };
        FATabBarEffect effectIn = new FATabBarEffect
        {
            FlexibleWidth = layoutFlexibleWidthIn,
            PaddingTop = paddingIn,
            TitlePreferedHeight = titlePreferedIn,
            TitleFlexibleHeight = t,
            TitleAlpha = t
        };
        buttonsTabItem[a].ApplyEffect(effectOut);
        buttonsTabItem[b].ApplyEffect(effectIn);

        selector.anchoredPosition = new Vector2(selectorAnchor, selector.anchoredPosition.y);
    }

    float SelectorPos(int page)
    {
        return (buttons[page].anchoredPosition.x - (buttons[page].sizeDelta.x * buttons[page].pivot.x)) + (selector.sizeDelta.x * selector.pivot.x);
    }

    void ResetTabItem(int tabToReset)
    {
        buttonsTabItem[tabToReset].ApplyEffect(effectOut);
    }

    void ResetTabItems(int selectedPage)
    {
        for (int i = 0; i < buttonsTabItem.Count; i++)
        {
            if (i == selectedPage)
            {
                buttonsTabItem[i].ApplyEffect(effectIn);
            }
            else
            {

                buttonsTabItem[i].ApplyEffect(effectOut);
            }
        }
    }
}
