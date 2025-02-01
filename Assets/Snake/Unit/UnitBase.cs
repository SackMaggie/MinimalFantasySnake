using Snake.Movement;
using UnityEngine;
using UnityEngine.Events;

namespace Snake.Unit
{
    /// <summary>
    /// Serve as a base class handle things that most unit will need
    /// </summary>
    public abstract class UnitBase : CustomMonoBehaviour, IUnit
    {
        [SerializeField] private int unitId;
        [SerializeField] private Vector2Int position;
        [SerializeField] private int health;
        [SerializeField] private int attack;
        [SerializeField] private int defense;
        [SerializeField] private Direction direction;
        [SerializeField] private bool isDead;

        private UnityEvent<(IUnit unit, IUnit killer)> onKilled = new UnityEvent<(IUnit unit, IUnit killer)>();
        protected IUnit killer = null;

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
        public virtual int UnitId
        {
            get => unitId; 
            set
            {
                unitId = value;
                gameObject.name = $"[{value}] {GetType().Name}";
            }
        }
        public virtual int Health { get => health; set => health = value; }
        public virtual int Attack { get => attack; set => attack = value; }
        public virtual int Defense { get => defense; set => defense = value; }
        public virtual Direction Direction { get => direction; set => direction = value; }
        public UnityEvent<(IUnit unit, IUnit killer)> OnKilled => onKilled;

        GameObject IUnit.GameObject => this == null ? null : gameObject;

        public bool IsDead { get => isDead; set => isDead = value; }

        public virtual void KillUnit(IUnit killer)
        {
            IUnit unit = this;
            this.killer = killer;
            GameObject gameObject = unit.GameObject;
            UnityEngine.GameObject.Destroy(gameObject);
        }

        protected override void OnDestroy()
        {
            OnKilled.Invoke((this, killer));
            base.OnDestroy();
        }
    }
}
