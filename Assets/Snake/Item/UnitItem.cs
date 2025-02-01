using Snake.Unit;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Snake.Item
{
    public class UnitItem : UnitBase, IItem
    {
        public ItemProperty ItemProperty { get; set; }

        public void OnPickUp(IUnit unit)
        {
            if (unit is null)
                throw new ArgumentNullException(nameof(unit));

            if (ItemProperty == null)
            {
                Debug.Log("Item have no property, do nothing");
                return;
            }

            if (this == null)
                return;

            foreach (ItemProperty.ItemEffect item in ItemProperty.itemEffects)
            {
                ApplyItemEffectToUnit(item, unit);
            }
        }

        private void ApplyItemEffectToUnit(ItemProperty.ItemEffect itemEffect, IUnit unit)
        {
            ItemProperty.UnitAttribute attribute = itemEffect.attribute;
            switch (itemEffect.commandType)
            {
                case ItemProperty.CommandType.Addition:
                    unit.SetAttributeValue(attribute, unit.GetAttributeValue(attribute) + itemEffect.value);
                    break;
                case ItemProperty.CommandType.Subtrack:
                    unit.SetAttributeValue(attribute, unit.GetAttributeValue(attribute) - itemEffect.value);
                    break;
                case ItemProperty.CommandType.Multiplier:
                    unit.SetAttributeValue(attribute, unit.GetAttributeValue(attribute) * itemEffect.value);
                    break;
                default:
                    throw new NotImplementedException(itemEffect.commandType.ToString());
            }
        }
    }

    public static class UnitItemExtension
    {
        public static float GetAttributeValue(this IUnit unit, ItemProperty.UnitAttribute unitAttribute) => unitAttribute switch
        {
            ItemProperty.UnitAttribute.Attack => unit.Attack,
            ItemProperty.UnitAttribute.Defense => unit.Defense,
            ItemProperty.UnitAttribute.Health => unit.Health,
            _ => throw new NotImplementedException(unitAttribute.ToString()),
        };

        public static void SetAttributeValue(this IUnit unit, ItemProperty.UnitAttribute unitAttribute, float value)
        {
            switch (unitAttribute)
            {
                case ItemProperty.UnitAttribute.Attack:
                    unit.Attack = Mathf.RoundToInt(value);
                    break;
                case ItemProperty.UnitAttribute.Defense:
                    unit.Defense = Mathf.RoundToInt(value);
                    break;
                case ItemProperty.UnitAttribute.Health:
                    unit.Health = Mathf.RoundToInt(value);
                    break;
                default:
                    throw new NotImplementedException(unitAttribute.ToString());
            }
        }
    }
}
