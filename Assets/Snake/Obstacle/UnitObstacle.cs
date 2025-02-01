using Snake.Unit;
using UnityEngine;

namespace Snake.Obstacle
{
    public class UnitObstacle : UnitBase, IUnitObstacle
    {
        [SerializeField] private Vector2Int size;

        public Vector2Int Size
        {
            get => size;
            set
            {
                transform.localScale = new Vector3(value.x, 1, value.y);
                size = value;
            }
        }
    }
}
