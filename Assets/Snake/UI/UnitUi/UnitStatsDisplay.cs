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
        [FormerlySerializedAs("itemEffect")][SerializeField] private StatsMapping itemEffectPrefab;
        [SerializeField] private Transform itemEffectRoot;
        [SerializeField] private Transform targetUnitTransform;
        private UnitType unitType;

        public IUnit TargetUnit { get; private set; }

        public void Init(IUnit unit)
        {
            TargetUnit = unit;
            targetUnitTransform = unit.GameObject.transform;
            unitType = unit.GetUnitType();

            health.gameObject.SetActive(unitType is UnitType.HERO or UnitType.MONSTER);
            attack.gameObject.SetActive(unitType is UnitType.HERO or UnitType.MONSTER);
            defense.gameObject.SetActive(unitType is UnitType.HERO or UnitType.MONSTER);
            itemEffectRoot.gameObject.SetActive(unitType is UnitType.ITEM);
            if (unit is IItem item)
            {
                foreach (Item.ItemProperty.ItemEffect itemEffect in item.ItemProperty.itemEffects)
                {
                    StatsMapping statsMapping = Instantiate(itemEffectPrefab, itemEffectRoot, false);
                    statsMapping.gameObject.SetActive(true);
                    statsMapping.valueText.text = itemEffect.GetDisplayText();
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            itemEffectPrefab.gameObject.SetActive(false);
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
