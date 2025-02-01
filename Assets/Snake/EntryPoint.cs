using Snake.Player;
using Snake.Unit;
using Snake.World;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Cinemachine;
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
        [SerializeField] private Arena arena;
        [SerializeField] private CinemachineTargetGroup targetGroup;
        [SerializeField] private SnakeChildObserver snakeChildObserver;

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
            gamePlayManager.arena = arena;
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
            if (this == null)
                return;
            try
            {
                switch (gameState)
                {
                    case GameState.None:
                        break;
                    case GameState.Initilizing:
                        DestroyGameMenu();
                        break;
                    case GameState.Playing:
                        SnakePlayer snakePlayer = gamePlayManager.SnakePlayer;
                        snakeChildObserver.Init(snakePlayer.ChildHero as ObservableCollection<IUnit>);
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
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }
}
