using Snake.World;
using System;
using System.Linq;
using UnityEngine;

namespace Snake
{
    public class EntryPoint : CustomMonoBehaviour
    {
        [SerializeField] private UnitView unitViewPrefab;
        [SerializeField] private GameMenu gameMenuPrefab;
        [SerializeField] private GameObject uiInputPrefab;
        [SerializeField] private GamePlayManager gamePlayManager;
        [SerializeField] private WorldGrid worldGrid;

        private Canvas canvasUI;

        private GameMenu gameMenu;
        private UnitView unitView;
        private GameObject uiInput;

        protected override void Start()
        {
            base.Start();
            Canvas[] allcanvas = FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
            canvasUI = allcanvas.FirstOrDefault(x => x.renderMode == RenderMode.ScreenSpaceOverlay);
            if (canvasUI == null)
                throw new Exception($"Create Canvas with renderMode = {RenderMode.ScreenSpaceOverlay}");
            CreateGameMenu();
        }

        private void CreateGameMenu()
        {
            if (gameMenu == null)
            {
                gameMenu = Instantiate(gameMenuPrefab, canvasUI.transform, false);
                gameMenu.startGameButton.onClick.AddListener(StartGame);
            }
        }

        private void StartGame()
        {
            if (unitView == null)
            {
                unitView = Instantiate(unitViewPrefab, canvasUI.transform, false);
                unitView.gamePlayManager = gamePlayManager;
            }
            if (uiInput == null)
                uiInput = Instantiate(uiInputPrefab, canvasUI.transform, false);
            gamePlayManager.OnGameStateChange.RemoveListener(OnGameStateChange);
            gamePlayManager.OnGameStateChange.AddListener(OnGameStateChange);
            gamePlayManager.InitilizeGame();
            DestroyGameMenu();
        }

        private void DestroyGameMenu()
        {
            if (gameMenu != null)
                Destroy(gameMenu.gameObject);
        }

        private void OnGameStateChange(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.None:
                    break;
                case GameState.Initilizing:
                    DestroyGameMenu();
                    break;
                case GameState.Playing:
                    break;
                case GameState.GameEnded:
                    CreateGameMenu();
                    if (uiInput != null)
                        Destroy(uiInput);
                    break;
                default:
                    break;
            }
        }
    }
}
