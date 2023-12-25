using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TileMatchGame.Controller;
using TileMatchGame.Enums;
using TileMatchGame.Manager;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;

public class MatchAreaManager : MonoBehaviour
{
    public SpriteRenderer MatchArea;
    [SerializeField] float widthOffset;
    [SerializeField] float heightOffset;
    List<MatchArea> areas;
    GameManager _gameManager;
    LevelManager _level;
    int _blastedItems;
    int _tileCapacity;

    public int BlastedItems => _blastedItems;

    public event Action itemsBlastedEvent;

    void Start()
    {
        itemsBlastedEvent += RecalculateAndArrangeItems;
    }

    public void Init(int tileCapacity)
    {
        _gameManager = GameManager.Instance;
        _level = _gameManager.Level;
        _tileCapacity = tileCapacity;
        areas = new List<MatchArea>(tileCapacity);
        MatchArea.size = new Vector2(tileCapacity + widthOffset, 1 + heightOffset);
        for (int i = 0; i < tileCapacity; i++)
        {
            var startX = (-1 * tileCapacity / 2f) + 1 / 2f; //tile length is 1
            var areaX = startX + i;

            var area = new GameObject("area" + i).AddComponent<MatchArea>();
            area.transform.SetParent(MatchArea.transform);
            area.transform.localPosition = Vector3.zero;
            area.transform.localScale = Vector3.one;
            area.IsEmpty = true;
            area.Index = i;
            area.transform.localPosition = new Vector2(areaX, 0);
            areas.Add(area);
        }
    }

    public int GetIndexOfAreaNeedToGo(ItemType itemType)
    {
        bool hasSameItemType = areas.Any(area => area.Item != null && area.Item.ItemType == itemType);

        if (hasSameItemType)
        {
            var lastSameItem = areas.Where(area => area.Item != null && area.Item.ItemType == itemType).Last();
            return lastSameItem.Index + 1;
        }
        else
        {
            var firstEmptyArea = areas.FirstOrDefault(area => area.IsEmpty);
            if (firstEmptyArea != null)
            {
                return firstEmptyArea.Index;
            }

            return -1;
        }
    }

    public List<MatchArea> GetAreasNeedToBeSwipeRight(int index)
    {
        return areas.Where(area => area.Item != null && area.Index > index).ToList();
    }

    public List<MatchArea> GetAreasNeedToBeSwipeLeft()
    {
        return areas.Where(area => area.Item != null).ToList();
    }

    public void SwipeRightAreas(List<MatchArea> areas)
    {
        for (int i = areas.Count - 1; i >= 0; i--)
        {
            var area = areas[i];
            var rightArea = GetAreaFromIndex(area.Index + 1);
            area.SwipeItemRight(rightArea);
        }
    }

    public void RecalculateAndArrangeItems()
    {
        var finalAreas = areas.Where(area => !area.IsEmpty && area.Item != null).ToList();

        if (finalAreas.Count == 0)
        {
            //there is no item to swipe left
            _gameManager.EnableInput();
        }

        var i = 0;
        foreach (var area in finalAreas)
        {
            if (area.Index == areas[i].Index)
            {
                //no left swiping because all items were on the left
                _gameManager.EnableInput();

                i++;
                continue;
            }

            area.Item.transform.DOMove(areas[i].GetPosition(), 0.1f).SetEase(Ease.Linear).OnComplete(() =>
            {
                //left swiping finished
                _gameManager.EnableInput();
            });

            areas[i].IsEmpty = false;
            areas[i].Item = area.Item;

            area.IsEmpty = true;
            area.Item = null;
            i++;
        }
    }

    public Vector3 GetAreaPositionFromIndex(int index)
    {
        var area = areas.SingleOrDefault(area => area.Index == index);
        if (area != null)
        {
            return area.transform.position;
        }

        return Vector3.zero;
    }

    public MatchArea GetAreaFromIndex(int index)
    {
        var area = areas.SingleOrDefault(area => area.Index == index);
        if (area != null)
        {
            return area;
        }

        return null;
    }

    public List<ItemController> GetItems()
    {
        var items = new List<ItemController>();
        foreach (var area in areas)
        {
            if (!area.IsEmpty)
            {
                items.Add(area.Item);
            }
        }

        return items;
    }

    public void BlastMatchedItems()
    {
        var items = GetItems();

        Dictionary<ItemType, List<ItemController>> itemTypeGroups = new();

        foreach (var item in items)
        {
            if (!itemTypeGroups.ContainsKey(item.ItemType))
            {
                itemTypeGroups[item.ItemType] = new List<ItemController>();
            }
            itemTypeGroups[item.ItemType].Add(item);
        }

        var blastSection = false;
        foreach (var itemTypeGroup in itemTypeGroups)
        {
            if (itemTypeGroup.Value.Count >= 3)
            {
                blastSection = true;
                BlastItems(itemTypeGroup.Key);
            }
        }
        if (!blastSection)
        {
            //no items blasted
            _gameManager.EnableInput();

            CheckGameOver();
        }
    }

    void CheckGameOver()
    {
        if (GetItems().Count >= _tileCapacity)
        {
            _level.GameOver();
        }
    }

    void BlastItems(ItemType itemType)
    {
        var i = 0;
        Action action = null;
        var blastAreas = areas.Where(area => area.Item != null && area.Item.ItemType == itemType).ToList();
        foreach (var area in blastAreas)
        {
            if (!area.IsEmpty)
            {
                if (i == blastAreas.Count - 1)
                {
                    action = itemsBlastedEvent;
                }

                _blastedItems++;
                area.Item.Blast(action);
                area.IsEmpty = true;
                i++;
            }
        }
    }

    public void Clear()
    {
        if (areas == null) return;

        foreach (var area in areas)
        {
            if (area.Item != null)
            {
                Destroy(area.Item.gameObject);
            }

            Destroy(area.gameObject);
        }
        areas.Clear();
        _blastedItems = 0;
    }
}