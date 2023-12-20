using TileMatchGame;
using TileMatchGame.Abstracts;
using TileMatchGame.Controller;
using TileMatchGame.Enums;
using TileMatchGame.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterLightBombAnim : MonoBehaviour, IBoosterAnim
{
    [SerializeField] LineRenderer linePrefab;
    [SerializeField] GameObject glowVFXPrefab;
    [SerializeField] float destroyDelay = .5f;

    LevelManager _level;
    List<LineRenderer> _lines = new List<LineRenderer>();
    List<GameObject> _particles = new List<GameObject>();
    List<Cell> _cellsToExplode = new List<Cell>();
    List<ItemController> _itemsToDestroy = new List<ItemController>();
    Transform _particleParent;
    int _blastedGoalItemCount;

    public void ExecuteSound()
    {
        var soundManager = GameManager.Instance.SoundManager;
        soundManager.PlaySound(soundManager.GameSounds.LightBombBoosterSound, 0.5f);
    }

    public void ExecuteAnim(Cell boosterCell, LevelManager level)
    {
        //_level = level;
        ////var goalItemType = _level.CurrentLevelData.GoalItemType;
        //FindCells(boosterCell);
        //PlayLightBombAnim(boosterCell);
        //StartCoroutine(FinishAnimDestroyItems(goalItemType));
    }

    IEnumerator FinishAnimDestroyItems(ItemType goalItemType)
    {
        yield return new WaitForSeconds(destroyDelay);

        foreach (var line in _lines)
        {
            Destroy(line.gameObject);
        }
        foreach (var particle in _particles)
        {
            Destroy(particle);
        }
        foreach (var item in _itemsToDestroy)
        {
            if (item == null) continue;
            if (item.ItemType == goalItemType)
            {
                _blastedGoalItemCount++;
            }
            item.Destroy();
        }
        UpdateGoalChart(goalItemType);
        Destroy(gameObject);
    }

    void UpdateGoalChart(ItemType goalItemType)
    {
        _level.UpdateLevelStats(goalItemType, _blastedGoalItemCount);
    }
}
