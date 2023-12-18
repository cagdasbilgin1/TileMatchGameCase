using UnityEngine;
using CollapseBlast.Enums;
using System.Collections.Generic;
using System.Linq;
using CollapseBlast.Manager;
using CollapseBlast.Controller;
using CollapseBlast.ScriptableObjects;

namespace CollapseBlast
{
    public class Board : MonoBehaviour
    {
        public Cell CellPrefab;
        public Transform CellsParent;
        public Transform ItemsParent;
        public Transform ParticlesAnimationsParent;
        public SpriteMask BoardMask;
        [HideInInspector] public List<Cell> Cells;
        GameManager _gameManager;
        SoundManager _soundManager;
        ItemManager _itemManager;
        MatchFinder _matchFinder;
        int _columns, _rows;
        float _distanceBetweenItems;
        float _usableScreenWidthRatio;
        float _usableScreenHeightRatio;
        List<TierData> _tierList;

        public void Init()
        {
            _gameManager = GameManager.Instance;
            _soundManager = _gameManager.SoundManager;
            var gamePlayCanvas = _gameManager.CanvasManager.GamePlayCanvas;
            _columns = _gameManager.Level.Columns;
            _rows = _gameManager.Level.Rows;
            _distanceBetweenItems = _gameManager.Level.DistanceBetweenItems;
            _usableScreenWidthRatio = _gameManager.Level.UsableScreenWidthRatio;
            _usableScreenHeightRatio = _gameManager.Level.UsableScreenHeightRatio;
            _itemManager = _gameManager.ItemManager;
            _tierList = _gameManager.Level.TierList;
            _matchFinder = new MatchFinder();
            _gameManager.metaSceneOpenedEvent += ClearObsoleteParticlesAnimations;

            CreateCells();
            InitCells();
            ArrangeBoardPosition();
            ArrangeBoardScale();
        }

        void ArrangeBoardPosition()
        {
            var xPos = _distanceBetweenItems * (_columns / -2f) + (_distanceBetweenItems / 2);
            var yPos = _distanceBetweenItems * (_rows / -2f) + (_distanceBetweenItems / 2);
            transform.localPosition = new Vector2(xPos, yPos);
        }

        void ArrangeBoardScale()
        {
            var camera = Camera.main;
            float aspect = camera.aspect;
            float worldHeight = camera.orthographicSize * 2;
            float worldWidth = worldHeight * aspect;
            var columnUnitWidth = worldWidth / (_columns * _distanceBetweenItems);

            transform.parent.localScale = new Vector2(columnUnitWidth, columnUnitWidth);

            var ItemEdgeUnit = Screen.width / _columns;
            float boardHeight = ItemEdgeUnit * _rows;
            float boardWidth = ItemEdgeUnit * _columns;
            while (boardHeight > Screen.height * _usableScreenHeightRatio || boardWidth > Screen.width * _usableScreenWidthRatio)
            {
                var scale = transform.parent.localScale.x;
                transform.parent.localScale = new Vector2(scale - scale * .05f, scale - scale * .05f);                
                boardHeight -= (boardHeight * .05f);
                boardWidth -= (boardWidth * .05f);
            }
        }

        private void CreateCells()
        {
            var i = 0;
            for (int tier = 1; tier <= _tierList.Count; tier++)
            {
                var tierTransform = new GameObject("Tier" + tier).transform;
                tierTransform.parent = CellsParent;
                for (var y = 0; y < _rows; y++)
                {
                    for (var x = 0; x < _columns; x++)
                    {
                        var cell = Instantiate(CellPrefab, Vector3.zero, Quaternion.identity, tierTransform);
                        cell.X = x;
                        cell.Y = y;
                        cell.Tier = tier;
                        Cells.Add(cell);
                        i++;
                    }
                }
            }
        }

        private void InitCells()
        {
            var i = 0;
            for (int tier = 1; tier <= _tierList.Count; tier++)
            {
                for (var y = 0; y < _rows; y++)
                {
                    for (var x = 0; x < _columns; x++)
                    {
                        Cells[i].Init(x, y, tier);
                        i++;
                    }
                }
            }
        }

        public void ClearElements()
        {
            foreach (var cell in Cells)
            {
                if (cell.Item != null) Destroy(cell.Item.gameObject);
                Destroy(cell.gameObject);
            }
            Cells.Clear();
        }

        public void ClearItems()
        {
            foreach (var cell in Cells)
            {
                if (cell.Item != null) Destroy(cell.Item.gameObject);
            }
        }

        public void CellTapped(Cell cell)
        {
            if (cell == null || cell.Item == null || cell.Item.IsNotClickable) return;

            var tappedItem = cell.Item;
            var tappedCellIsBooster = tappedItem.IsBooster;
            var tappedCellTypeIndex = tappedItem.TypeIndex;
            DestroyMatchedItems(cell);

            if (!tappedCellIsBooster && tappedCellTypeIndex > 0) //create booster
            {
                cell.Item = GameManager.Instance.ItemManager.CreateItem(ItemType.Booster, cell.transform.localPosition, tappedCellTypeIndex - 1);
            }
            else if (tappedCellIsBooster)
            {
                var boosterIndex = tappedItem.TypeIndex;

                _itemManager.ExecuteBooster(boosterIndex, cell);
            }
        }

        private void DestroyMatchedItems(Cell cell)
        {
            var itemType = cell.Item.ItemType;
            if (itemType == ItemType.Booster)
            {
                return;
            }

            var partOfMatchedCells = _matchFinder.FindMatch(cell, itemType);

            if (partOfMatchedCells == null) return;

            _gameManager.Level.UpdateLevelStats(itemType, partOfMatchedCells.Count);
            _soundManager.PlaySound(_soundManager.GameSounds.ItemBlastSound);

            foreach (var matchedCell in partOfMatchedCells)
            {
                matchedCell.Item.Destroy();
            }
        }

        public Cell GetNeighbourWithDirection(Cell cell, Direction direction)
        {
            //var x = cell.X;
            //var y = cell.Y;

            //switch (direction)
            //{
            //    case Direction.Up:
            //        y += 1;
            //        break;
            //    case Direction.Down:
            //        y -= 1;
            //        break;
            //    case Direction.Right:
            //        x += 1;
            //        break;
            //    case Direction.Left:
            //        x -= 1;
            //        break;
            //    case Direction.UpRight:
            //        x += 1;
            //        y += 1;
            //        break;
            //    case Direction.UpLeft:
            //        x -= 1;
            //        y += 1;
            //        break;
            //    case Direction.DownRight:
            //        x += 1;
            //        y -= 1;
            //        break;
            //    case Direction.DownLeft:
            //        x -= 1;
            //        y -= 1;
            //        break;
            //}

            //if (x >= _columns || x < 0 || y >= _rows || y < 0) return null;

            //return GetCell(x, y, 1);
            return null;
        }

        Cell GetCell(int x, int y, int tier)
        {
            return Cells.Single(cell => cell.X == x && cell.Y == y && cell.Tier == tier);
        }

        public Cell GetRandomCellAtBoard()
        {
            //var x = Random.Range(0, _columns);
            //var y = Random.Range(0, _rows);

            //var cell = GetCell(x, y, 1);
            //if (cell.Item != null && !cell.Item.IsBooster)
            //{
            //    return cell;
            //}
            //else
            //{
            //    return GetRandomCellAtBoard();
            //}
            return null;
        }

        public List<Cell> GetCellsWithItemType(ItemType itemType)
        {
            return Cells.Where(cell => cell.Item?.ItemType == itemType).ToList();
        }

        void ClearObsoleteParticlesAnimations()
        {
            foreach (Transform obsoleteParticle in ParticlesAnimationsParent)
            {
                Destroy(obsoleteParticle.gameObject);
            }
        }
    }
}
