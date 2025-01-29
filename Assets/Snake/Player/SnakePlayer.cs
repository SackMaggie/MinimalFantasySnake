using Snake.Unit;
using System.Collections.Generic;
using UnityEngine;

namespace Snake.Player
{
    public class SnakePlayer : UnitBase, Unit.IPlayer
    {
        public static SnakePlayer Instance;
        private readonly List<IUnit> childHero = new List<IUnit>();
        public IList<IUnit> ChildHero => childHero;

        protected override void Start()
        {
            base.Start();
        }
    }
}