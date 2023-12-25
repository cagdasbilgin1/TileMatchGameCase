using System.Collections.Generic;
using TileMatchGame.Enums;
using TileMatchGame.ScriptableObjects;
using UnityEngine;

namespace TileMatchGame.Manager
{
    public class HintManager
    {
        private Board _board;

        public void Init()
        {
            _board = GameManager.Instance.Board;
        }

        public void TickUpdate()
        {
            ArrangeItemIcon();
        }

        public void ArrangeItemIcon()
        {
            var cells = _board.Cells;

            foreach ( var cell in cells )
            {
                var item = cell.Item;

                if (item == null) continue;

                item.ArrangeSorting();
                item.ArrangeFruitSprite();
            }
        }
    }
}


