using UnityEngine;

namespace Snake.Player
{
    //TODO: Add movement control
    public class SnakePlayer : CustomMonoBehaviour, Unit.IPlayer
    {
        public static SnakePlayer Instance;

        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public Vector2Int Position { get; set; }

        protected override void Start()
        {
            base.Start();
        }
    }

    public enum Direction
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
    }
}