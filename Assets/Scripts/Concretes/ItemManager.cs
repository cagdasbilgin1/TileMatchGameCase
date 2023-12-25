using UnityEngine;
using TileMatchGame.Enums;
using TileMatchGame.ScriptableObjects;
using TileMatchGame.Controller;

namespace TileMatchGame.Manager
{
    public class ItemManager : MonoBehaviour
    {
        [SerializeField] ItemTypesData _itemTypesData;
        [SerializeField] ItemController _itemPrefab;

        public Sprite GetItemSprite(ItemType fruitItemType)
        {
            return _itemTypesData.FruitSprites[(int)fruitItemType];
        }

        public ItemController CreateItem(ItemType itemType, Vector3 itemSpawnPos)
        {
            var item = Instantiate(_itemPrefab, Vector3.zero, Quaternion.identity).GetComponent<ItemController>();
            item.Init(itemType, itemSpawnPos);
            return item;
        }
    }
}


