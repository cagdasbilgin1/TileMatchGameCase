using System.Collections.Generic;
using TileMatchGame.Enums;
using TileMatchGame.ScriptableObjects;
using UnityEngine;

namespace TileMatchGame.Manager
{
    public class HintManager
    {
        private int _columns, _rows;
        private Board _board;
        private LevelDataSO _currentLevelData;

        public void Init()
        {
            _board = GameManager.Instance.Board;
            _currentLevelData = GameManager.Instance.Level.CurrentLevelData;
            _columns = _currentLevelData.Columns;
            _rows = _currentLevelData.Rows;
        }

        public void TickUpdate()
        {
            ArrangeItemIcon();
        }

        public void ArrangeItemIcon()
        {
            int rows = _rows;
            int cols = _columns;
            var cells = _board.Cells;

            //var matchedCellInfo = GetMatchedCellInfos();


            foreach ( var cell in cells )
            {
                var item = cell.Item;
                var x = cell.Position.x;
                var y = cell.Position.y;

                if (item == null) continue;

                item.ArrangeSorting();

                item.ArrangeFruitSprite();

            }
        }        

        private void FillMatchedCellInfos(List<Cell> partOfMatchedCells, int[,] matchedCellInfos)
        {
            if (partOfMatchedCells == null) return;
            var size = partOfMatchedCells.Count;
            foreach (var cell in partOfMatchedCells)
            {
                matchedCellInfos[cell.Position.x, cell.Position.y] = size;
            }
        }
    }
}


