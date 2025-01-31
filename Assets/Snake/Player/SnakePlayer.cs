using Snake.Movement;
using Snake.Unit;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Snake.Player
{
    public class SnakePlayer : UnitBase, Unit.IPlayer
    {
        public static SnakePlayer Instance;
        private readonly List<IUnit> childHero = new List<IUnit>();
        public IList<IUnit> ChildHero => childHero;
        public IUnit CurrentHero { get => ChildHero.FirstOrDefault(); set => throw new NotImplementedException(); }

        public override int Health
        {
            get => CurrentHero.Health;
            set
            {
                CurrentHero.Health = value;
                base.Health = value;
            }
        }

        public override int Attack
        {
            get => CurrentHero.Attack;
            set
            {
                CurrentHero.Attack = value;
                base.Attack = value;
            }
        }

        public override int Defense
        {
            get => CurrentHero.Defense;
            set
            {
                CurrentHero.Defense = value;
                base.Defense = value;
            }
        }

        public override Vector2Int Position
        {
            get => CurrentHero.Position;
            set
            {
                CurrentHero.Position = value;
                base.Position = value;
            }
        }

        public override Direction Direction
        {
            get => CurrentHero.Direction;
            set
            {
                CurrentHero.Direction = value;
                base.Direction = value;
            }
        }

        [SerializeField] private int childHeroCount;
        [SerializeField] private List<GameObject> childHeroObject = new List<GameObject>();

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
            childHeroObject = ChildHero.Select(x => x.GameObject).ToList();
        }

        public void KillUnit()
        {
            throw new Exception("The player should not get killed, try kill the hero line to trigger game over instead");
        }
#endif
    }
}