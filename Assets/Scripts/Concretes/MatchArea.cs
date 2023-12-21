using System.Collections;
using System.Collections.Generic;
using TileMatchGame.Controller;
using UnityEngine;

public class MatchArea : MonoBehaviour
{
    bool _isEmpty;
    ItemController _item;

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
}
