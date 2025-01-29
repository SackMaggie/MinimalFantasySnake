using Snake.Unit;
using UnityEngine;

namespace Snake.Player
{
    //TODO: Add movement control
    public class SnakePlayer : UnitBase, Unit.IPlayer
    {
        public static SnakePlayer Instance;

        protected override void Start()
        {
            base.Start();
        }
    }
}