using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TileMatchGame.Controller;
using TileMatchGame.Enums;
using UnityEngine;

public class MatchArea : MonoBehaviour
{
    bool _isEmpty;
    [SerializeField] ItemController _item;
    [SerializeField] int _index;

    public bool IsEmpty
    {
        get { return _isEmpty; }
        set { _isEmpty = value; }
    }
    public ItemController Item
    {
        get { return _item; }
        set { _item = value; }
    }

    public int Index
    {
        get { return _index; }
        set { _index = value; }
    }

    public void SwipeItemRight(MatchArea rightArea)
    {
        if (Item == null) return;

        Item.transform.DOMove(rightArea.GetPosition(), 0.1f).SetEase(Ease.Linear);
        rightArea.Item = Item;
        rightArea.IsEmpty = false;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }
}
