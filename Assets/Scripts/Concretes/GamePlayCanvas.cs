using TileMatchGame.Manager;
using TileMatchGame.ScriptableObjects;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TileMatchGame.Canvas
{
    public class GamePlayCanvas : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _levelGoalCounterText;
        [SerializeField] TextMeshProUGUI _levelMovesCounterText;
        [SerializeField] Image _levelGoalItemTypeImage;
        [SerializeField] GameObject _levelUpUI;
        [SerializeField] GameObject _gameOverUI;
        LevelManager _level;

        private void Awake()
        {
            var gameManager = GameManager.Instance;
            _level = gameManager.Level;

            _level.OnLevelUpEvent += ShowLevelUpUI;
            _level.OnGameOverEvent += ShowGameOverUI;
        }

        public void OnBackToMetaButtonClick()
        {
            GameManager.Instance.ToggleScene();
        }

        void ShowLevelUpUI()
        {
            var soundManager = GameManager.Instance.SoundManager;
            soundManager.PlayMusic(soundManager.GameSounds.LevelWin, 1, false);
            _levelUpUI.SetActive(true);
        }

        void DismissLevelUpUI()
        {
            _levelUpUI.SetActive(false);
        }

        void ShowGameOverUI()
        {
            var soundManager = GameManager.Instance.SoundManager;
            soundManager.PlayMusic(soundManager.GameSounds.LevelLose, 1, false);
            _gameOverUI.SetActive(true);
        }

        void DismissGameOverUI()
        {
            _gameOverUI.SetActive(false);
        }

        public void OnClaimButtonClick()
        {
            GameManager.Instance.ToggleScene();

            DismissLevelUpUI();
            
            var gameManager = GameManager.Instance;
            gameManager.Board.ClearElements();
            gameManager.InitGame();
        }

        public void OnGoHomeButtonClick()
        {
            GameManager.Instance.ToggleScene();

            DismissGameOverUI();

            var gameManager = GameManager.Instance;
            gameManager.Board.ClearElements();
            gameManager.InitGame();
        }
    }
}