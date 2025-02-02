using Snake.Player;
using Snake.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Snake.Settings
{
    public class GameSettingUi : CustomMonoBehaviour
    {
        public GameSetting gameSetting;
        public SettingFieldInt settingFieldIntPrefab;
        public SettingFieldFloat settingFieldFloatPrefab;

        public SettingFieldVector2Int settingFieldVector2IntPrefab;
        public SettingFieldVector2 settingFieldVector2Prefab;

        public SettingGroup settingGroupPrefab;

        public RectTransform container;

        protected override void Start()
        {
            base.Start();

            CreateSettingVector2Int(() => gameSetting.boardSize, (value) => gameSetting.boardSize = value, nameof(gameSetting.boardSize), container);
            SettingGroup spawnSettingGroup = CreateSettingGroup($"Spawn Setting", container);
            CreateSpawnSetting(UnitType.HERO, spawnSettingGroup.container);
            CreateSpawnSetting(UnitType.MONSTER, spawnSettingGroup.container);
            CreateSpawnSetting(UnitType.ITEM, spawnSettingGroup.container);
            CreateSpawnSetting(UnitType.OBSTACLE, spawnSettingGroup.container);
            SettingGroup unitStatSettingGroup = CreateSettingGroup($"Unit Stat Setting", container);
            CreateStatSettingMonster("Hero", gameSetting.heroStats, unitStatSettingGroup.container);
            CreateStatSettingMonster("Monster", gameSetting.monsterStats, unitStatSettingGroup.container);

            void CreateSpawnSetting(UnitType unitType, RectTransform container)
            {
                GameSetting.SpawnSetting spawnSetting = gameSetting.spawnSettings.First(x => x.unitType == unitType);
                SettingGroup settingGroup = CreateSettingGroup($"{unitType}", container);
                CreateSettingInt(() => spawnSetting.maxSpawnCount, (value) => spawnSetting.maxSpawnCount = value, $"{nameof(spawnSetting.maxSpawnCount)}", settingGroup.container);
                CreateSettingInt(() => spawnSetting.minSpawnCount, (value) => spawnSetting.minSpawnCount = value, $"{nameof(spawnSetting.minSpawnCount)}", settingGroup.container);
                CreateSettingFloat(() => spawnSetting.spawnChance, (value) => spawnSetting.spawnChance = value, $"{nameof(spawnSetting.spawnChance)}", settingGroup.container);
            }

            void CreateStatSettingMonster(string fieldName, List<GameSetting.StatsSetting> statList, RectTransform container)
            {
                SettingGroup settingGroup = CreateSettingGroup(fieldName, container);
                CreateStatSetting(UnitClassEnum.Warrior, statList, settingGroup.container);
                CreateStatSetting(UnitClassEnum.Rogue, statList, settingGroup.container);
                CreateStatSetting(UnitClassEnum.Wizard, statList, settingGroup.container);
            }

            void CreateStatSetting(UnitClassEnum unitClassEnum, List<GameSetting.StatsSetting> statList, RectTransform container)
            {
                GameSetting.StatsSetting statSetting = statList.First(x => x.unitClass == unitClassEnum);
                SettingGroup settingGroup = CreateSettingGroup($"{unitClassEnum}", container);
                CreateSettingVector2Int(() => statSetting.attackRange, (value) => statSetting.attackRange = value, $"Attack", settingGroup.container);
                CreateSettingVector2Int(() => statSetting.healthRange, (value) => statSetting.healthRange = value, $"Health", settingGroup.container);
                CreateSettingVector2Int(() => statSetting.defenseRange, (value) => statSetting.defenseRange = value, $"Defense", settingGroup.container);
            }
        }

        private SettingFieldVector2Int CreateSettingVector2Int(Func<Vector2Int> ReadValueFunc, Action<Vector2Int> WriteValueFunc, string fieldName, RectTransform container = null)
        {
            SettingFieldVector2Int settingField = Instantiate(settingFieldVector2IntPrefab, container == null ? this.container : container, false);
            settingField.ReadValueFunc = ReadValueFunc;
            settingField.WriteValueFunc = WriteValueFunc;
            settingField.SetFieldName(fieldName);
            return settingField;
        }

        private SettingFieldVector2 CreateSettingVector2(Func<Vector2> ReadValueFunc, Action<Vector2> WriteValueFunc, string fieldName, RectTransform container = null)
        {
            SettingFieldVector2 settingField = Instantiate(settingFieldVector2Prefab, container == null ? this.container : container, false);
            settingField.ReadValueFunc = ReadValueFunc;
            settingField.WriteValueFunc = WriteValueFunc;
            settingField.SetFieldName(fieldName);
            return settingField;
        }

        private SettingFieldInt CreateSettingInt(Func<int> ReadValueFunc, Action<int> WriteValueFunc, string fieldName, RectTransform container = null)
        {
            SettingFieldInt settingField = Instantiate(settingFieldIntPrefab, container == null ? this.container : container, false);
            settingField.ReadValueFunc = ReadValueFunc;
            settingField.WriteValueFunc = WriteValueFunc;
            settingField.SetFieldName(fieldName);
            return settingField;
        }

        private SettingFieldFloat CreateSettingFloat(Func<float> ReadValueFunc, Action<float> WriteValueFunc, string fieldName, RectTransform container = null)
        {
            SettingFieldFloat settingField = Instantiate(settingFieldFloatPrefab, container == null ? this.container : container, false);
            settingField.ReadValueFunc = ReadValueFunc;
            settingField.WriteValueFunc = WriteValueFunc;
            settingField.SetFieldName(fieldName);
            return settingField;
        }

        private SettingGroup CreateSettingGroup(string fieldName, RectTransform container = null)
        {
            SettingGroup settingField = Instantiate(settingGroupPrefab, container == null ? this.container : container, false);
            settingField.SetFieldName(fieldName);
            return settingField;
        }
    }
}
