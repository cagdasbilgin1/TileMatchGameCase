using TileMatchGame.Abstracts;
using TileMatchGame.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace TileMatchGame.Manager
{
    public class GameManager : SingletonMonoBehaviour<GameManager>
    {
        public Board Board;
        public event Action gamePlaySceneOpenedEvent;
        public event Action metaSceneOpenedEvent;

        [SerializeField] List<GameObject> _gamePlaySceneElements;
        [SerializeField] List<GameObject> _metaSceneElements;        

        [HideInInspector] public LevelManager Level;
        [HideInInspector] public ItemManager ItemManager;
        [HideInInspector] public TouchManager TouchManager;
        [HideInInspector] public HintManager HintManager;
        [HideInInspector] public CanvasManager CanvasManager;
        [HideInInspector] public SoundManager SoundManager;
        [HideInInspector] public MatchAreaManager MatchAreaManager;

        void Awake()
        {
            SetSingletonThisGameObject(this);

            Application.targetFrameRate = 120;

            HintManager = new HintManager();
            Level = GetComponent<LevelManager>();
            ItemManager = GetComponent<ItemManager>();
            TouchManager = GetComponent<TouchManager>();
            CanvasManager = GetComponent<CanvasManager>();
            SoundManager = GetComponent<SoundManager>();
            MatchAreaManager = GetComponent<MatchAreaManager>();
            InitGame();

            Level.OnLevelUpEvent += DisableInput;
            Level.OnGameOverEvent += DisableInput;
        }

        public void InitGame()
        {
            Level.Init();
            Board.Init();
            HintManager.Init();
        }

        private void Update()
        {
            HintManager.TickUpdate();
        }

        public void EnableInput()
        {
            TouchManager.enabled = true;
        }

        public void DisableInput()
        {
            TouchManager.enabled = false;
        }

        public void ToggleScene()
        {
            var isGameplayActive = false;
            foreach (var gameplaySceneElement in _gamePlaySceneElements)
            {
                gameplaySceneElement.SetActive(!gameplaySceneElement.activeInHierarchy);
                isGameplayActive = gameplaySceneElement.activeInHierarchy;
            }

            if (isGameplayActive)
            {
                EnableInput();
                gamePlaySceneOpenedEvent?.Invoke();
            }
            else
            {
                DisableInput();
                metaSceneOpenedEvent?.Invoke();
            }
        }
    }
}