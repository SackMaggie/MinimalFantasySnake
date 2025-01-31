using Snake.Unit;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Snake
{
    public class UnitStatsDisplay : CustomMonoBehaviour
    {
        public new Camera camera;
        public Vector2 offset;
        [SerializeField] private StatsMapping health;
        [SerializeField] private StatsMapping attack;
        [SerializeField] private StatsMapping defense;
        [SerializeField] private Transform targetUnitTransform;
        public IUnit TargetUnit { get; private set; }

        public void Init(IUnit unit)
        {
            TargetUnit = unit;
            targetUnitTransform = unit.GameObject.transform;
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
            health.valueText.text = $"{TargetUnit.Health}";
            attack.valueText.text = $"{TargetUnit.Attack}";
            defense.valueText.text = $"{TargetUnit.Defense}";
        }

        [Serializable]
        internal class StatsMapping
        {
            public Image icon;
            public TMP_Text valueText;
        }
    }
}
