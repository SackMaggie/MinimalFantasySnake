using Snake.Player;
using Snake.Unit;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Snake.UI
{
    public class UnitStatsDisplay : CustomMonoBehaviour
    {
        public new Camera camera;
        public Vector2 offset;
        [SerializeField] private StatsMapping health;
        [SerializeField] private StatsMapping attack;
        [SerializeField] private StatsMapping defense;
        [SerializeField] private Transform itemEffectRoot;
        [SerializeField] private Transform statsRoot;
        [SerializeField] private Transform targetUnitTransform;
        private UnitType unitType;

        public IUnit TargetUnit { get; private set; }

        public void Init(IUnit unit)
        {
            TargetUnit = unit;
            targetUnitTransform = unit.GameObject.transform;
            unitType = unit.GetUnitType();

            statsRoot.gameObject.SetActive(unitType is UnitType.HERO or UnitType.MONSTER);
            itemEffectRoot.gameObject.SetActive(unitType is UnitType.ITEM);
            if (unit is IItem item)
            {
                foreach (Item.ItemProperty.ItemEffect itemEffect in item.ItemProperty.itemEffects)
                {
                    StatsMapping statsMapping;
                    switch (itemEffect.attribute)
                    {
                        case Item.ItemProperty.UnitAttribute.Attack:
                            statsMapping = Instantiate(attack, itemEffectRoot, false);
                            break;
                        case Item.ItemProperty.UnitAttribute.Defense:
                            statsMapping = Instantiate(defense, itemEffectRoot, false);
                            break;
                        case Item.ItemProperty.UnitAttribute.Health:
                            statsMapping = Instantiate(health, itemEffectRoot, false);
                            break;
                        default:
                            continue;
                    }
                    statsMapping.gameObject.SetActive(true);
                    statsMapping.valueText.text = itemEffect.GetDisplayText();
                }
            }
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
            if (camera == null)
                camera = Camera.main;
            if (targetUnitTransform == null)
                return;
            transform.position = RectTransformUtility.WorldToScreenPoint(camera, targetUnitTransform.position) + offset;
            if (TargetUnit == null)
                return;
            UpdateUIValue();
        }

        private void UpdateUIValue()
        {
            switch (unitType)
            {
                case UnitType.HERO:
                case UnitType.MONSTER:
                    health.valueText.text = $"{TargetUnit.Health}";
                    attack.valueText.text = $"{TargetUnit.Attack}";
                    defense.valueText.text = $"{TargetUnit.Defense}";
                    break;
                case UnitType.ITEM:
                    //value not changes
                    break;
                default:
                    break;
            }
        }
    }
}
