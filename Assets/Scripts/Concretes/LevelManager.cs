using TileMatchGame.Enums;
using TileMatchGame.ScriptableObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TileMatchGame.Manager
{
    public class LevelManager : MonoBehaviour
    {
        const string keyLevelIndex = "LevelIndex";
        public List<LevelDataSO> levels;

        int _rows, _columns;
        float _distanceBetweenItems;
        float _usableScreenWidthRatio;
        float _usableScreenHeightRatio;
        int _tierCount;
        List<TierData> _tierList;
        int _levelIndex;
        int _minimumBlastableMatch;
        int _goalCount;
        int _moveCount;
        int _allItemsCount;
        LevelDataSO _currentLevelData;
        Board _board;
        ItemManager _itemManager;
        //MatchAreaManager _matchAreaManager;
        GameManager _gameManager;
        List<ItemType> _itemTypes;
        List<LevelItem> levelItemDatas;

        public int GoalCount => _goalCount;
        public int MoveCount => _moveCount;
        public int Rows => _rows;
        public int Columns => _columns;
        public float DistanceBetweenItems => _distanceBetweenItems;
        public float UsableScreenWidthRatio => _usableScreenWidthRatio;
        public float UsableScreenHeightRatio => _usableScreenHeightRatio;
        public List<TierData> TierList => _tierList;
        public int LevelIndex => _levelIndex;
        public int MinimumBlastableMatch => _minimumBlastableMatch;
        public LevelDataSO CurrentLevelData => _currentLevelData;

        public event Action OnLevelUpEvent;
        public event Action OnGameOverEvent;
        public event Action OnLevelStatsUpdateEvent;

        void Start()
        {
            _gameManager.metaSceneOpenedEvent += ResetLevel;
            _gameManager.MatchAreaManager.itemsBlastedEvent += LevelUp;
        }

        public void Init()
        {
            _gameManager = GameManager.Instance;
            _levelIndex = PlayerPrefs.GetInt(keyLevelIndex, 0);
            _board = _gameManager.Board;
            _itemManager = _gameManager.ItemManager;
            _currentLevelData = levels[_levelIndex];
            _rows = _currentLevelData.Rows;
            _columns = _currentLevelData.Columns;
            _distanceBetweenItems = _currentLevelData.DistanceBetweenItems;
            _usableScreenWidthRatio = _currentLevelData.UsableScreenWidthRatio;
            _usableScreenHeightRatio = _currentLevelData.UsableScreenHeightRatio;
            _tierList = _currentLevelData.TierList;
            _minimumBlastableMatch = _currentLevelData.MinimumBlastableCell;
            _allItemsCount = GetTotalItemCount();
        }

        public void LevelUp()
        {
            if (_gameManager.MatchAreaManager.BlastedItems >= _allItemsCount)
            {
                _levelIndex++;
                _levelIndex = _levelIndex < levels.Count ? _levelIndex : 0;
                _currentLevelData = levels[_levelIndex];
                PlayerPrefs.SetInt(keyLevelIndex, _levelIndex);
                OnLevelUpEvent?.Invoke();
            }
        }

        public void GameOver()
        {
            OnGameOverEvent?.Invoke();
        }

        public void ResetLevel()
        {
            _board.ClearItems();
            _board.Init();
        }

        public void UpdateLevelStats(ItemType blastedItemsType, int blastedItemCount)
        {
            _moveCount--;
            //if (_currentLevelData.GoalItemType == blastedItemsType)
            //{
            //    _goalCount -= blastedItemCount;
            //}

            if (_goalCount <= 0)
            {
                _goalCount = 0;
                LevelUp();
            }
            else if (_moveCount <= 0)
            {
                _moveCount = 0;
                GameOver();
            }

            OnLevelStatsUpdateEvent?.Invoke();
        }

        ItemType DefineItemType(int i)
        {
            if (levelItemDatas.Any())
            {
                i = levelItemDatas[i].colorIndex;
            }
            else
            {
                i = UnityEngine.Random.Range(0, _itemTypes.Count);
            }

            return _itemTypes[i];
        }

        public int GetTotalItemCount()
        {
            var totalItemCount = 0;

            foreach (var tier in _currentLevelData.TierList)
            {
                totalItemCount += tier.Cards.Count;
            }

            return totalItemCount;
        }
    }
}
