using System;

namespace Snake.Item
{
    [Serializable]
    public class ItemProperty
    {
        [Serializable]
        public class ItemEffect
        {
            public CommandType commandType;
            public UnitAttribute attribute;
            public float value;
        }


        public enum UnitAttribute
        {
            Attack,
            Defense,
            Health,
        }

        public enum CommandType
        {
            Addition,
            Subtrack,
            Multiplier,
        }
    }
}
