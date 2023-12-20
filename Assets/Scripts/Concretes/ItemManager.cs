﻿using UnityEngine;
using TileMatchGame.Enums;
using TileMatchGame.ScriptableObjects;
using TileMatchGame.Controller;
using TileMatchGame.Abstracts;

namespace TileMatchGame.Manager
{
    public class FallAnimData
    {
        public FallAnimData(float startVelocity, float acceleration, float maxVelocity)
        {
            StartVelocity = startVelocity;
            Acceleration = acceleration;
            MaxVelocity = maxVelocity;
        }

        public float StartVelocity;
        public float Acceleration;
        public float MaxVelocity;
    }
    public class ItemManager : MonoBehaviour
    {
        [SerializeField] float _startVelocity = 0.4f;
        [SerializeField] float _acceleration = 0.3f;
        [SerializeField] float _maxVelocity = 12f;
        [SerializeField] ItemTypesData _itemTypesData;
        [SerializeField] BoosterAnimationSO _boosterAnimations;
        [SerializeField] ItemController _itemPrefab;
        //Transform _itemsParent;
        Transform _particlesAnimationsParent;
        LevelManager _levelManager;

        [HideInInspector] public FallAnimData FallAnimData;

        public void Init()
        {
            var gameManager = GameManager.Instance;
            //_itemsParent = gameManager.Board.ItemsParent;
            _particlesAnimationsParent = gameManager.Board.ParticlesAnimationsParent;
            _levelManager = gameManager.Level;
            FallAnimData = new FallAnimData(_startVelocity, _acceleration, _maxVelocity);
        }

        public Sprite GetItemSprite(ItemType itemType, int index)
        {
            switch (itemType)
            {
                case ItemType.Apple:
                    return _itemTypesData.RedBoxes[index];
                case ItemType.Orange:
                    return _itemTypesData.GreenBoxes[index];
                case ItemType.Strawberry:
                    return _itemTypesData.BlueBoxes[index];
                case ItemType.Avocado:
                    return _itemTypesData.YellowBoxes[index];
                case ItemType.Coconut:
                    return _itemTypesData.PurpleBoxes[index];
                case ItemType.Banana:
                    return _itemTypesData.PinkBoxes[index];
                case ItemType.Cherry:
                    return _itemTypesData.Boosters[index];
            }
            return null;
        }

        public ItemController CreateItem(ItemType itemType, Vector3 itemSpawnPos)
        {
            var item = Instantiate(_itemPrefab, Vector3.zero, Quaternion.identity).GetComponent<ItemController>();
            item.Init(itemType, itemSpawnPos);
            return item;
        }

        //public void ExecuteBooster(int boosterIndex, Cell boosterCell)
        //{
        //    var booster = Instantiate(_boosterAnimations.BoosterAnimations[boosterIndex], boosterCell.transform.position, Quaternion.identity, _particlesAnimationsParent).GetComponent<IBoosterAnim>();

        //    booster.ExecuteSound();
        //    booster.ExecuteAnim(boosterCell, _levelManager);
        //}
    }
}


