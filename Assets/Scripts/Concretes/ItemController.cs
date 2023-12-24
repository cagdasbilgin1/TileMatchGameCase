using UnityEngine;
using TileMatchGame.Enums;
using TileMatchGame.Manager;
using DG.Tweening;
using UnityEditor.Rendering;
using System;

namespace TileMatchGame.Controller
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] SpriteRenderer _fruitSpriteRenderer;
        [SerializeField] Animator _animator;
        ItemManager _itemManager;
        Cell _cell;
        FallAnimation _fallAnimation;
        ItemType _itemType;
        bool _isNotClickable;

        public ItemType ItemType => _itemType;
        public bool IsNotClickable => _isNotClickable; 

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

        public void ArrangeFruitSprite()
        {
            _fruitSpriteRenderer.sprite = _itemManager.GetItemSprite(_itemType);
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
            _spriteRenderer.sortingOrder = _cell.Tier * 10;
            _fruitSpriteRenderer.sortingOrder = _cell.Tier * 10 + 1;
        }

        public void ArrangeClickable(bool clickable)
        {
            _isNotClickable = !clickable;
        }

        private void Update()
        {
            //_fallAnimation.TickUpdate();
        }

        public void Blast(Action itemsBlastedEvent)
        {
            transform.DOScale(Vector3.zero, .2f).OnComplete(() =>
            {
                if (itemsBlastedEvent != null)
                {
                    itemsBlastedEvent?.Invoke();
                }
                Destroy(gameObject);
            });
        }
    }
}
