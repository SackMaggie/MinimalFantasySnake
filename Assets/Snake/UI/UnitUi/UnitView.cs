using Snake.Unit;
using System;
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
            gamePlayManager.OnUnitSpawn.AddListener(OnUnitSpawn);
            gamePlayManager.OnUnitKill.AddListener(OnUnitKill);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            gamePlayManager.OnUnitSpawn.RemoveListener(OnUnitSpawn);
            gamePlayManager.OnUnitKill.RemoveListener(OnUnitKill);
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
