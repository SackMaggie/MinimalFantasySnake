﻿using Snake.Movement;
using UnityEngine;
using UnityEngine.Events;

namespace Snake.Unit
{
    /// <summary>
    /// Serve as a base class handle things that most unit will need
    /// </summary>
    public abstract class UnitBase : CustomMonoBehaviour, IUnit
    {
        [SerializeField] private Vector2Int position;
        [SerializeField] private int health;
        [SerializeField] private int attack;
        [SerializeField] private int defense;
        [SerializeField] private Direction direction;

        private UnityEvent<(IUnit unit, IUnit killer)> onKilled = new UnityEvent<(IUnit unit, IUnit killer)>();

        public virtual Vector2Int Position
        {
            get => position;
            set
            {
                position = value;
                // since our value is vector2 it's intended to use y value as Z axis
                // we have no dynamic height
                transform.position = new Vector3(position.x, transform.position.y, position.y);
            }
        }
        public virtual int Health { get => health; set => health = value; }
        public virtual int Attack { get => attack; set => attack = value; }
        public virtual int Defense { get => defense; set => defense = value; }
        public virtual Direction Direction { get => direction; set => direction = value; }
        public UnityEvent<(IUnit unit, IUnit killer)> OnKilled => onKilled;

        GameObject IUnit.GameObject => this == null ? null : gameObject;

        public virtual void KillUnit(IUnit killer)
        {
            IUnit unit = this;
            GameObject gameObject = unit.GameObject;
            Debug.LogWarning($"Kill unit {name} health={Health}", gameObject);
            OnKilled.Invoke((this, killer));
            UnityEngine.GameObject.Destroy(gameObject);
        }
    }
}
