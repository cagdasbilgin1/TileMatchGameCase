using System.Collections.Generic;
using UnityEngine;

namespace TileMatchGame.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ItemTypesData", menuName = "TileMatchGame/ItemTypesData", order = 1)]
    public class ItemTypesData : ScriptableObject
    {
        [SerializeField] List<Sprite> fruitSprites;

        public List<Sprite> FruitSprites => fruitSprites;
    }
}

