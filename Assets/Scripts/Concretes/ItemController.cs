using UnityEngine;
using TileMatchGame.Enums;
using TileMatchGame.Manager;
using DG.Tweening;
using UnityEditor.Rendering;
using System;
using Unity.VisualScripting;

namespace TileMatchGame.Controller
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] SpriteRenderer _spriteRenderer;
        [SerializeField] SpriteRenderer _fruitSpriteRenderer;
        [SerializeField] Animator _animator;
        ItemManager _itemManager;
        Cell _cell;
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
            transform.position = pos;
        }

        public void ArrangeFruitSprite()
        {
            _fruitSpriteRenderer.sprite = _itemManager.GetItemSprite(_itemType);
        }

        public void SetTouchInactive()
        {
            if (_isNotClickable) return;
            Cell.GetComponent<BoxCollider2D>().enabled = false;
            _isNotClickable = true;
            SwitchToInactiveColor();
        }

        public void SwitchToInactiveColor()
        {
            Color currentTileColor = _spriteRenderer.color;
            Color currentFruitColor = _spriteRenderer.color;
            float darkenAmount = .8f;
            Color darkenedColor = Color.Lerp(Color.white, Color.black, darkenAmount);
            _spriteRenderer.color = Color.Lerp(currentTileColor, darkenedColor, darkenAmount);
            _fruitSpriteRenderer.color = Color.Lerp(currentFruitColor, darkenedColor, darkenAmount);
        }

        public void SetTouchActive()
        {
            if (!_isNotClickable) return;
            Cell.GetComponent<BoxCollider2D>().enabled = true;
            _isNotClickable = false;
            SwitchToOriginalColor();
        }

        public void SwitchToOriginalColor()
        {
            _spriteRenderer.color = Color.white;
            _fruitSpriteRenderer.color = Color.white;
        }

        public void Destroy()
        {
            if (this == null) return;

            _cell.Item = null;
            _cell = null;
            Destroy(gameObject);
        }

        public void ArrangeSorting()
        {
            _spriteRenderer.sortingOrder = _cell.Tier * 100 + _cell.Position.y;
            _fruitSpriteRenderer.sortingOrder = _spriteRenderer.sortingOrder + 1;
        }

        public void ArrangeClickable(bool clickable)
        {
            _isNotClickable = !clickable;
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
