using Snake.Movement;
using Snake.Unit;
using System.Collections.Generic;
using UnityEngine;

namespace Snake.Obstacle
{
    public class UnitObstacle : UnitBase, IUnitObstacle
    {
        [SerializeField] private List<Vector2Int> subPosition = new List<Vector2Int>();
        [SerializeField] private Vector2Int size;
        [SerializeField] private Direction horizontalDirection;
        [SerializeField] private Direction verticleDirection;
        [SerializeField] private Transform childRoot;

        public override Direction Direction { get => direction; set => direction = value; }

        public Vector2Int Size
        {
            get => size;
            set
            {
                size = value;
                UpdateChildSize();
            }
        }

        public Direction HorizontalDirection { get => horizontalDirection; set => horizontalDirection = value; }
        public Direction VerticleDirection { get => verticleDirection; set => verticleDirection = value; }
        public IList<Vector2Int> SubPosition => subPosition;

        [ContextMenu("TestSetSize")]
        private void TestSetSize()
        {
            Size = size;
        }

        private void UpdateChildSize()
        {
            int isRight = (HorizontalDirection == Direction.RIGHT ? 1 : -1);
            int isUp = (VerticleDirection == Direction.UP ? 1 : -1);
            childRoot.rotation = Quaternion.AngleAxis(0, Vector3.up);
            childRoot.localPosition = new Vector3((size.x - 1) * 0.5f * isRight, 0, (size.y - 1) * 0.5f * isUp);
            childRoot.localScale = new Vector3(size.x, 1, size.y);
        }
    }
}
