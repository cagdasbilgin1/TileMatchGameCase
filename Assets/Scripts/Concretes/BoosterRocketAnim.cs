using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using TileMatchGame.Enums;
using TileMatchGame.Manager;
using TileMatchGame.Abstracts;

namespace TileMatchGame.Controller
{
    public class BoosterRocketAnim : MonoBehaviour, IBoosterAnim
    {
        [SerializeField] Transform piece1, piece2;
        [SerializeField] ParticleSystem particle1, particle2;
        Board _board;
        LevelManager _level;
        Camera _camera;
        Vector3 piece1Pos, piece2Pos;
        bool _isHorizontal;
        bool _isPlaying;
        int xOffset = Screen.width * 2;
        int yOffset = Screen.height;
        List<Cell> _cellsToExplode = new List<Cell>();
        int _blastedGoalItemCount;
        bool _piece1OutOfScreen => piece1Pos.x < -xOffset || piece1Pos.x > Screen.width + xOffset || piece1Pos.y < -yOffset || piece1Pos.y > Screen.height + yOffset;
        bool _piece2OutOfScreen => piece2Pos.x < -xOffset || piece2Pos.x > Screen.width + xOffset || piece2Pos.y < -yOffset || piece2Pos.y > Screen.height + yOffset;

        public void ExecuteSound()
        {
            var soundManager = GameManager.Instance.SoundManager;
            soundManager.PlaySound(soundManager.GameSounds.RocketBoosterSound, .3f);
        }

        public void ExecuteAnim(Cell boosterCell, LevelManager level)
        {
            //_level = level;
            //var goalItemType = _level.CurrentLevelData.GoalItemType;
            //FindCells(boosterCell, out _isHorizontal);
            //DestroyCellItems(goalItemType);
            //UpdateGoalChart(goalItemType);
            //PlayRocketAnim(_isHorizontal);
        }

        void Update()
        {
            if (!_isPlaying) return;

            piece1Pos = _camera.WorldToScreenPoint(piece1.position);
            piece2Pos = _camera.WorldToScreenPoint(piece2.position);

            if (_piece1OutOfScreen && _piece2OutOfScreen)
            {
                Destroy(gameObject);
            }
        }

        void UpdateGoalChart(ItemType goalItemType)
        {
            _level.UpdateLevelStats(goalItemType, _blastedGoalItemCount);
        }

        void OnDestroy()
        {
            piece1.DOKill();
            piece2.DOKill();
        }
    }
}