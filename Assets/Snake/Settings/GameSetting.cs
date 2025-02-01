using Snake.Player;
using Snake.Unit;
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
        [SerializeField] private List<StatsSetting> heroStats;
        [SerializeField] private List<StatsSetting> monsterStats;
        [SerializeField] private Vector2Int boardSize = new Vector2Int(16, 16);
        [SerializeField] private List<ItemBinding> spawnableItems;
        [SerializeField] private List<Vector2Int> obstacleSizes;

        private static StatsSetting itemStat = new StatsSetting()
        {
            attackRange = Vector2Int.zero,
            defenseRange = Vector2Int.zero,
            healthRange = Vector2Int.one,
        };

        private static StatsSetting obstacleStat = new StatsSetting()
        {
            attackRange = Vector2Int.zero,
            defenseRange = Vector2Int.zero,
            healthRange = Vector2Int.one,
        };

        public SpawnSetting GetSpawnSetting(UnitType unitType)
        {
            return spawnSettings.FirstOrDefault(x => x.unitType == unitType);
        }

        public StatsSetting GetStatsSetting(UnitType unitType, UnitClassEnum unitClass) => unitType switch
        {
            UnitType.HERO => heroStats.First(x => x.unitClass == unitClass),
            UnitType.MONSTER => monsterStats.First(x => x.unitClass == unitClass),
            UnitType.ITEM => itemStat,
            UnitType.OBSTACLE => obstacleStat,
            _ => throw new NotImplementedException(unitType.ToString()),
        };


        public Vector2Int GetBoardSize() => boardSize;

        public ItemBinding GetRandomItem()
        {
            int totalWeight = spawnableItems.Sum(x => x.spawnWeight);
            int total = 0;
            int value = Random.Range(0, totalWeight + 1);
            for (int i = 0; i < spawnableItems.Count; i++)
            {
                ItemBinding itemBinding = spawnableItems[i];
                total += itemBinding.spawnWeight;
                if (total >= value)
                    return itemBinding;
            }
            Debug.Log("Error");
            return spawnableItems.FirstOrDefault();
        }

        [ContextMenu("TestRandom")]
        private void TestRandomItem()
        {
            List<int> spawnedId = new List<int>();
            for (int i = 0; i < 1000; i++)
            {
                ItemBinding itemBinding = GetRandomItem();
                spawnedId.Add(itemBinding.id);
            }
            Dictionary<int, int> dictionary = spawnedId.GroupBy(x => x).ToDictionary(k => k.Key, v => v.Count());
            Debug.Log(string.Join("\n", dictionary.Select(x => $"id {x.Key} {x.Value} ~ {x.Value / 10}%")));
        }

        public Vector2Int GetRandomObstaclesSize()
        {
            return obstacleSizes[Random.Range(0, obstacleSizes.Count)];
        }

        [Serializable]
        public class SpawnSetting
        {
            public UnitType unitType;
            [Min(1)] public int maxSpawnCount;
            [Min(0)] public int minSpawnCount;
            [Range(0, 1)] public float spawnChance;
        }

        [Serializable]
        public class StatsSetting
        {
            public UnitClassEnum unitClass;
            public Vector2Int healthRange;
            public Vector2Int attackRange;
            public Vector2Int defenseRange;

            public int Health => Random.Range(healthRange.x, healthRange.y);
            public int Attack => Random.Range(attackRange.x, attackRange.y);
            public int Defense => Random.Range(defenseRange.x, defenseRange.y);
        }

        [Serializable]
        public class ItemBinding
        {
            public int id;
            /// <summary>
            /// Weight of the spawn, not the chances
            /// </summary>
            public ushort spawnWeight = 10;
            public Item.ItemProperty property;
            public GameObject objectBinding;
        }
    }
}
