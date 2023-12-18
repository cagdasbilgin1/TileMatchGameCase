using CollapseBlast.Enums;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CollapseBlast.ScriptableObjects
{
    enum CardType
    {
        Apple,
        Banana,
        Avocado,
        Cherry,
        Coconut,
        Grape,
        Kiwi,
        Lemon,
        Orange,
        Watermelon,
        Strawberry,
        Mango,
        Pineapple
    }

    [System.Serializable]
    public class Card
    {
        [SerializeField] Vector2 position;
        [SerializeField] CardType cardType;
        
    }

    [System.Serializable]
    public class TierData
    {
        [SerializeField] List<Card> cards;
    }

    [CreateAssetMenu(fileName = "LevenConfigSO", menuName = "CollapseBlast/LevelConfigSO", order = 0)]
    public class LevelDataSO : ScriptableObject
    {
        [Tooltip("M")][SerializeField] int rows;
        [Tooltip("N")][SerializeField] int columns;
        [SerializeField] float distanceBetweenItems;
        [SerializeField] float usableScreenWidthRatio;
        [SerializeField] float usableScreenHeightRatio;
        [Tooltip("K")][SerializeField] List<ItemType> colors;
        [Tooltip("A")][SerializeField] int firstSpecialIconTypeThreshold;
        [Tooltip("B")][SerializeField] int secondSpecialIconTypeThreshold;
        [Tooltip("C")][SerializeField] int thirdSpecialIconTypeThreshold;
        [Tooltip("if empty, colors are chosen randomly")][SerializeField] TextAsset levelJson;
        [SerializeField] int minimumBlastableMatch;
        [SerializeField] ItemType goalItemType;
        [SerializeField] int goalCount;
        [SerializeField] int movesCount;
        [SerializeField] int tierCount;
        [SerializeField] List<TierData> tierList = new List<TierData>();

        public int Rows => rows;
        public int Columns => columns;
        public float DistanceBetweenItems => distanceBetweenItems;
        public float UsableScreenWidthRatio => usableScreenWidthRatio;
        public float UsableScreenHeightRatio => usableScreenHeightRatio;
        public List<ItemType> ItemTypes => colors;
        public int FirstSpecialIconTypeThreshold => firstSpecialIconTypeThreshold;
        public int SecondSpecialIconTypeThreshold => secondSpecialIconTypeThreshold;
        public int ThirdSpecialIconTypeThreshold => thirdSpecialIconTypeThreshold;
        public TextAsset LevelJson => levelJson;
        public int MinimumBlastableCell => minimumBlastableMatch;
        public ItemType GoalItemType => goalItemType;
        public int GoalCount => goalCount;
        public int MovesCount => movesCount;
        public int TierCount => tierCount;
        public List<TierData> TierList => tierList;
    }
}

