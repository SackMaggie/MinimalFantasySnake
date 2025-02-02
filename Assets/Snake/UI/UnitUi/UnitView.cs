using Snake.UI;
using Snake.Unit;
using System.Collections.Generic;
using UnityEngine;

namespace Snake
{
    public class UnitView : CustomMonoBehaviour
    {
        [SerializeField] private UnitStatsDisplay unitStatsDisplayPrefab;
        [SerializeField] private RectTransform container;

        public GamePlayManager gamePlayManager;
        private Dictionary<IUnit, UnitStatsDisplay> unitStatsDict = new Dictionary<IUnit, UnitStatsDisplay>();

        protected override void Start()
        {
            base.Start();
            CreateUiForEachUnit();
            Debug.Log("UnitView.Start");
            //for handling late hook
            foreach (GameState item in gamePlayManager.GameStateList)
                OnGameStateChange(item);
            gamePlayManager.OnGameStateChange.AddListener(OnGameStateChange);
            gamePlayManager.OnUnitSpawn.AddListener(OnUnitSpawn);
            gamePlayManager.OnUnitKill.AddListener(OnUnitKill);
        }

        private void OnGameStateChange(GameState gameState)
        {
            switch (gameState)
            {
                case GameState.CleanUp:
                    foreach (KeyValuePair<IUnit, UnitStatsDisplay> item in unitStatsDict)
                    {
                        if (item.Value == null)
                            continue;
                        Destroy(item.Value.gameObject);
                    }
                    unitStatsDict.Clear();
                    break;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            gamePlayManager.OnUnitSpawn.RemoveListener(OnUnitSpawn);
            gamePlayManager.OnUnitKill.RemoveListener(OnUnitKill);
        }

        private void CreateUiForEachUnit()
        {
            foreach (IUnit item in gamePlayManager.GetAllUnit())
                OnUnitSpawn(item);
        }

        private void OnUnitKill(IUnit unit)
        {
            if (unit == null)
            {
                Debug.LogError("Killed unit is null ??");
                return;
            }
            if (unitStatsDict.Remove(unit, out UnitStatsDisplay unitStatsDisplay))
            {
                Destroy(unitStatsDisplay.gameObject);
            }
        }

        private void OnUnitSpawn(IUnit unit)
        {
            if (unit == null)
            {
                Debug.LogError("Spawned unit is null ??");
                return;
            }
            if (!unitStatsDict.TryGetValue(unit, out UnitStatsDisplay unitStatsDisplay))
            {
                unitStatsDisplay = Instantiate(unitStatsDisplayPrefab, container, false);
                unitStatsDict[unit] = unitStatsDisplay;
                unitStatsDisplay.Init(unit);
            }
        }
    }
}
