using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TileMatchGame.Controller;
using TileMatchGame.Enums;
using UnityEngine;

public class MatchAreaManager : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] float widthOffset;
    [SerializeField] float heightOffset;
    List<MatchArea> areas;

    public void Init(int tileCapacity)
    {
        areas = new List<MatchArea>(tileCapacity);
        _spriteRenderer.size = new Vector2(tileCapacity + widthOffset, 1 + heightOffset);
        for (int i = 0; i < tileCapacity; i++)
        {
            var startX = (-1 * tileCapacity / 2f) + 1 / 2f; //tile length is 1
            var areaX = startX + i;

            var area = new GameObject("area" + i).AddComponent<MatchArea>();
            area.transform.parent = transform;
            area.transform.localPosition = Vector3.zero;
            area.transform.localScale = Vector3.one;
            area.IsEmpty = true;
            area.Index = i;
            area.transform.localPosition = new Vector2(areaX, 0);
            areas.Add(area);
        }
    }

    public Vector3 GetFirstEmptyAreaPosition()
    {
        var firstEmptyArea = areas.FirstOrDefault(area => area.IsEmpty);
        if (firstEmptyArea != null)
        {
            return firstEmptyArea.transform.position;
        }
        else
        {
            //there is no empty area
            return Vector3.zero;
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

    public void SwipeRightAreas(List<MatchArea> areas)
    {
        for (int i = areas.Count - 1; i >= 0; i--)
        {
            var area = areas[i];
            var rightArea = GetAreaFromIndex(area.Index + 1);
            area.SwipeItemRight(rightArea);
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
}