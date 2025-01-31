using Snake.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Snake
{
    [CreateAssetMenu(menuName = "Snake")]
    public class GameSetting : ScriptableObject
    {
        [SerializeField] private List<SpawnSetting> spawnSettings;
        [SerializeField] private StatsSetting heroStat;
        [SerializeField] private StatsSetting monsterStat;

        public SpawnSetting GetSpawnSetting(UnitType unitType)
        {
            return spawnSettings.FirstOrDefault(x => x.unitType == unitType);
        }

        public StatsSetting GetStatsSetting(UnitType unitType) => unitType switch
        {
            UnitType.HERO => heroStat,
            UnitType.MONSTER => monsterStat,
            _ => throw new NotImplementedException(unitType.ToString()),
        };

        [Serializable]
        public class SpawnSetting
        {
            public UnitType unitType;
            [Min(1)] public int maxSpawnCount;
            [Range(0, 1)] public float spawnChance;
        }

        [Serializable]
        public class StatsSetting
        {
            public Vector2Int healthRange;
            public Vector2Int attackRange;
            public Vector2Int defenseRange;

            public int Health => Random.Range(healthRange.x, healthRange.y);
            public int Attack => Random.Range(attackRange.x, attackRange.y);
            public int Defense => Random.Range(defenseRange.x, defenseRange.y);
        }
    }
}
