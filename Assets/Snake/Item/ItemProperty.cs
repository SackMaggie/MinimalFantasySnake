using System;
using System.Collections.Generic;

namespace Snake.Item
{
    [Serializable]
    public class ItemProperty
    {
        public List<ItemEffect> itemEffects;

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
