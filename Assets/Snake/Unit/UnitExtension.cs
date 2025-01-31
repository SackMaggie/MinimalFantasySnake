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
    }
}
