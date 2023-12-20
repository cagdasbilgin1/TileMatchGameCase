using UnityEngine;
using TileMatchGame.Enums;
using TileMatchGame.Manager;

namespace TileMatchGame.Controller
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] Animator _animator;
        ItemManager _itemManager;
        Cell _cell;
        FallAnimation _fallAnimation;
        ItemType _itemType;
        int _typeIndex;
        bool _isNotClickable;

        public ItemType ItemType => _itemType;
        //public bool IsBooster => _itemType == ItemType.Booster;
        public bool IsNotClickable => _isNotClickable;
        public int TypeIndex { get { return _typeIndex; } set { _typeIndex = value; } }
        public bool IsHorizontalRocketBooster;


        public Cell Cell
        {
            get { return _cell; }
            set
            {
                if (_cell == value) return;

                var oldCell = _cell;
                _cell = value;

                if (oldCell != null && oldCell.Item == this)
                {
                    oldCell.Item = null;
                }

                if (value != null)
                {
                    value.Item = this;
                    gameObject.name = $"Item {_cell.Position.x}:{_cell.Position.y} [{_cell.Tier}]";
                }
            }
        }

        public void Init(ItemType itemType, Vector3 pos)
        {
            _itemType = itemType;
            _itemManager = GameManager.Instance.ItemManager;
            //_fallAnimation = new FallAnimation(_itemManager.FallAnimData, this);
            transform.position = pos;
        }

        public void ChangeSprite(int typeIndex)
        {
            //if (ItemType == ItemType.Booster) return;
            _spriteRenderer.sprite = _itemManager.GetItemSprite(_itemType, typeIndex);
        }

        public void Destroy()
        {
            if (this == null) return;

            _cell.Item = null;
            _cell = null;
            Destroy(gameObject);
        }

        public void Fall()
        {
            _fallAnimation.FallTo(_cell.GetFallTargetCell());
        }

        public void ArrangeSorting()
        {
            _spriteRenderer.sortingOrder = _cell.Tier;
        }

        public void ArrangeClickable(bool clickable)
        {
            _isNotClickable = !clickable;
        }

        private void Update()
        {
            //_fallAnimation.TickUpdate();
        }
    }
}
