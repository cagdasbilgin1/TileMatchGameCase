using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TileMatchGame.Controller;
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
            area.transform.localPosition = new Vector2(areaX, 0);
            areas.Add(area);
        }
    }

    public void FillFirstEmptyArea(ItemController item)
    {
        var firstEmptyArea = areas.FirstOrDefault(area => area.IsEmpty);
        if (firstEmptyArea != null)
        {
            firstEmptyArea.IsEmpty = false;
            firstEmptyArea.Item = item;
        }
        else
        {

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
}