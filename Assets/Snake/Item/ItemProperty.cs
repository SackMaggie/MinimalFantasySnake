using System;
using System.Collections.Generic;
using UnityEngine;

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

            public string GetDisplayText() => commandType switch
            {
                CommandType.Addition => value >= 0 ? $"+{value}" : $"{value}",
                CommandType.Subtrack => value >= 0 ? $"-{value}" : $"+{Mathf.Abs(value)}",
                CommandType.Multiplier => $"X{value}",
                _ => throw new NotImplementedException(commandType.ToString()),
            };
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
