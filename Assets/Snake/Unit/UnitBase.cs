using Snake.Movement;
using UnityEngine;

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
        public Direction Direction { get => direction; set => direction = value; }
        GameObject IUnit.GameObject => gameObject;
    }
}
