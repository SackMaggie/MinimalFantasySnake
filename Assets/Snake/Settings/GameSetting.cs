using Snake.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snake
{
    [CreateAssetMenu(menuName = "Snake")]
    public class GameSetting : ScriptableObject
    {
        [SerializeField] private List<SpawnSetting> spawnSettings;

        public SpawnSetting GetSpawnSetting(UnitType unitType)
        {
            return spawnSettings.FirstOrDefault(x => x.unitType == unitType);
        }

        [Serializable]
        public class SpawnSetting
        {
            public UnitType unitType;
            [Min(1)] public int maxSpawnCount;
            [Range(0, 1)] public float spawnChance;
        }
    }
}
