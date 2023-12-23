using TileMatchGame.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TileMatchGame.ScriptableObjects
{
    [System.Serializable]
    public class Card
    {
        [SerializeField] Vector2Int position;
        [SerializeField] ItemType itemType;

        public ItemType ItemType => itemType;
        public Vector2Int Position => position;
    }

    [System.Serializable]
    public class TierData
    {
        [SerializeField] List<Card> cards;

        public List<Card> Cards => cards;
    }

    [CreateAssetMenu(fileName = "LevenConfigSO", menuName = "TileMatchGame/LevelConfigSO", order = 0)]
    public class LevelDataSO : ScriptableObject
    {
        [SerializeField] int rows;
        [SerializeField] int columns;
        [SerializeField] float distanceBetweenItems;
        [SerializeField] float usableScreenWidthRatio;
        [SerializeField] float usableScreenHeightRatio;
        [SerializeField] int minimumBlastableMatch;
        [SerializeField] int matchAreaTileCapacity;
        [SerializeField] List<TierData> tierList = new List<TierData>();

        public int Rows => rows;
        public int Columns => columns;
        public float DistanceBetweenItems => distanceBetweenItems;
        public float UsableScreenWidthRatio => usableScreenWidthRatio;
        public float UsableScreenHeightRatio => usableScreenHeightRatio;
        public int MinimumBlastableCell => minimumBlastableMatch;
        public int MatchAreaTileCapacity => matchAreaTileCapacity;
        public List<TierData> TierList => tierList;
    }
}

