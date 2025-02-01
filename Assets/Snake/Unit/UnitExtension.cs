using Snake.Player;
using System;

namespace Snake.Unit
{
    public static class UnitExtension
    {
        public static void ApplyStats(this IUnit unit, GameSetting.StatsSetting statsSetting)
        {
            unit.Health = statsSetting.Health;
            unit.Attack = statsSetting.Attack;
            unit.Defense = statsSetting.Defense;
        }

        public static UnitType GetUnitType(this IUnit unit) => unit switch
        {
            IMonster => UnitType.MONSTER,
            IHeros => UnitType.HERO,
            _ => throw new NotImplementedException(unit.GetType().ToString()),
        };

        public static void ApplyItemProperty(this IItem item, Item.ItemProperty itemProperty)
        {
            item.ItemProperty = itemProperty;
        }
    }
}
