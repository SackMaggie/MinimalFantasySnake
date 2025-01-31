using Snake.Movement;
using Snake.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snake.Player
{
    public class SnakePlayer : CustomMonoBehaviour, Unit.IPlayer
    {
        public static SnakePlayer Instance;
        private readonly List<IUnit> childHero = new List<IUnit>();
        public IList<IUnit> ChildHero => childHero;
        public IUnit CurrentHero { get => ChildHero.First(); set=> throw new NotImplementedException(); }
        public int Health { get => CurrentHero.Health; set => CurrentHero.Health =value; }
        public int Attack { get => CurrentHero.Attack; set => CurrentHero.Attack = value; }
        public int Defense { get => CurrentHero.Defense; set => CurrentHero.Defense = value; }
        public Vector2Int Position { get => CurrentHero.Position; set => CurrentHero.Position = value; }
        public Direction Direction { get => CurrentHero.Direction; set => CurrentHero.Direction = value; }
        GameObject IUnit.GameObject => gameObject;

        [SerializeField] private int childHeroCount;

        protected override void Start()
        {
            base.Start();
        }

#if UNITY_EDITOR
        protected override void Update()
        {
            base.Update();
            //TODO: Remove this for debug
            childHeroCount = ChildHero.Count;
        }
#endif
    }
}