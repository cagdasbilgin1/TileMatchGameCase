﻿using UnityEngine;
using TileMatchGame.Enums;
using System.Collections.Generic;
using System.Linq;
using TileMatchGame.Manager;
using TileMatchGame.Controller;
using TileMatchGame.ScriptableObjects;
using UnityEditor.Rendering.LookDev;
using System.Collections;

namespace TileMatchGame
{
    public class Board : MonoBehaviour
    {
        public GameObject MatchAreaPrefab;
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
        [SerializeField] float TopAreaRatio;
        [SerializeField] float MiddleAreaRatio;
        [SerializeField] float BottomAreaRatio;

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
            FillBoard();

            ArrangeBoardPosition();
            ArrangeBoardScale();
            InitMatchArea();

            //GetScreenSectionMiddleWorldPosition();
        }

        void ArrangeBoardPosition()
        {
            //board placed in the center of the screen
            var xPos = _distanceBetweenItems * (_columns / -2f) + (_distanceBetweenItems / 2);
            var yPos = _distanceBetweenItems * (_rows / -2f) + (_distanceBetweenItems / 2);
            transform.localPosition = new Vector2(xPos, yPos);

            //board parent placed at the specified point
            transform.parent.position = GetScreenSectionWorldPosition(2); // 2 means middle area
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

        void InitMatchArea()
        {
            var matchArea = Instantiate(MatchAreaPrefab, Vector3.zero, Quaternion.identity, transform);
            matchArea.GetComponent<SpriteRenderer>().size = new Vector2(6, 1);
            matchArea.transform.parent = GameManager.Instance.transform;
            matchArea.transform.position = GetScreenSectionWorldPosition(1);
        }

        private void CreateCellDummy()
        {
            //delete
            var tierTransform = new GameObject("TierNull").transform;
            tierTransform.parent = CellsParent;
            for (var y = 0; y < _rows; y++)
            {
                for (var x = 0; x < _columns; x++)
                {
                    var cell = Instantiate(CellPrefab, Vector3.zero, Quaternion.identity, tierTransform);
                    cell.Position.x = x;
                    cell.Position.y = y;
                    cell.Tier = 0;
                    cell.Init();
                }
            }
        }

        private void CreateCells()
        {
            //CreateCellDummy(); //delete test
            ///
            var tierIndex = 0;
            foreach (var tier in _tierList)
            {
                var cellTierTransform = new GameObject("Tier" + tierIndex).transform;
                cellTierTransform.parent = CellsParent;

                foreach (var item in tier.Cards)
                {
                    var cell = Instantiate(CellPrefab, Vector3.zero, Quaternion.identity, cellTierTransform);
                    cell.Position.x = item.Position.x;
                    cell.Position.y = item.Position.y;
                    cell.Tier = tierIndex;
                    Cells.Add(cell);
                }
                tierIndex++;
            }
        }

        private void InitCells()
        {
            foreach (var cell in Cells)
            {
                cell.Init();
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
                if (cell.Item != null)
                {
                    cell.Item.transform.parent = null;
                    Destroy(cell.Item.gameObject);
                }
            }
        }

        public void CellTapped(Cell cell)
        {
            if (cell == null || cell.Item == null || cell.Item.IsNotClickable) return;

            var tappedItem = cell.Item;
            //var tappedCellIsBooster = tappedItem.IsBooster;
            var tappedCellTypeIndex = tappedItem.TypeIndex;
            DestroyMatchedItems(cell);

            //if (!tappedCellIsBooster && tappedCellTypeIndex > 0) //create booster
            //{
            //    cell.Item = GameManager.Instance.ItemManager.CreateItem(ItemType.Booster, cell.transform.localPosition, tappedCellTypeIndex - 1);
            //}
            //else if (tappedCellIsBooster)
            //{
            //    var boosterIndex = tappedItem.TypeIndex;

            //    _itemManager.ExecuteBooster(boosterIndex, cell);
            //}
        }

        private void DestroyMatchedItems(Cell cell)
        {
            var itemType = cell.Item.ItemType;
            //if (itemType == ItemType.Booster)
            //{
            //    return;
            //}

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

        public Cell GetCell(Vector2Int position, int tier)
        {
            return Cells.SingleOrDefault(cell => cell.Position.x == position.x && cell.Position.y == position.y && cell.Tier == tier);
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

        public void FillBoard()
        {
            var tierIndex = 0;
            foreach (var tier in _tierList)
            {
                Transform _itemTierTransform = ItemsParent.Find("Tier" + tierIndex);
                if (_itemTierTransform == null)
                {
                    _itemTierTransform = new GameObject("Tier" + tierIndex).transform;
                }

                _itemTierTransform.parent = ItemsParent;
                foreach (var item in tier.Cards)
                {
                    var cell = GetCell(item.Position, tierIndex);
                    if (cell != null)
                    {
                        cell.Item = _itemManager.CreateItem(item.ItemType, cell.transform.position);
                        cell.Item.transform.parent = _itemTierTransform;
                        cell.Item.transform.localScale = Vector3.one;
                    }
                }
                tierIndex++;
            }
        }

        Vector2 GetScreenSectionWorldPosition(int areaIndex)
        {
            var camera = Camera.main;
            float screenHeight = Screen.height;

            float topHeight = screenHeight * BottomAreaRatio;
            float middleHeight = screenHeight * MiddleAreaRatio;
            float bottomHeight = screenHeight * TopAreaRatio;

            float topPosition = topHeight / 2f;
            float middlePosition = topHeight + middleHeight / 2f;
            float bottomPosition = topHeight + middleHeight + bottomHeight / 2f;

            //calculate world positions
            switch (areaIndex)
            {
                case 1: return camera.ScreenToWorldPoint(new Vector2(Screen.width / 2f, topPosition));
                case 2: return camera.ScreenToWorldPoint(new Vector2(Screen.width / 2f, middlePosition));
                case 3: return camera.ScreenToWorldPoint(new Vector2(Screen.width / 2f, bottomPosition));
                default: return Vector2.zero;
            }
        }
    }
}