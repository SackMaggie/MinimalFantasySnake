
using Snake.Unit;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Snake.World
{
    public class WorldGrid : CustomMonoBehaviour
    {
        [SerializeField] private List<TestUnit> units = new List<TestUnit>();
        private Vector2Int gridSize;

        public IUnit[,] UnitGrid { get; private set; }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();

            //TODO: Remove this, for debugging only
#if UNITY_EDITOR
            if (UnitGrid != null)
            {
                units.Clear();
                foreach (IUnit item in UnitGrid)
                {
                    if (item is IUnit unit)
                    {
                        units.Add(new TestUnit()
                        {
                            Position = unit.Position,
                            Attack = unit.Attack,
                            Defense = unit.Defense,
                            Health = unit.Health,
                        });
                    }
                }
            }
#endif
        }

        internal void CreateGrid(Vector2Int gridSize)
        {
            this.gridSize = gridSize;
            UnitGrid = new IUnit[gridSize.x, gridSize.y];
            //Test fill the grid
            /*for (int x = 0; x < gridSize.x; x++)
            {
                for (int y = 0; y < gridSize.y; y++)
                {
                    UnitGrid[x, y] = new TestUnit()
                    {
                        Position = new Vector2Int(x, y),
                    };
                }
            }*/
        }

        internal Vector2Int GetEmptyPosition()
        {
            Vector2Int position;
            ///For fail safe in do-while loop
            int i = 0;
            if (gridSize.x <= 0 || gridSize.y <= 0)
                throw new Exception($"{nameof(gridSize)} should not be below or equal to zero");
            int maxRetry = gridSize.x * gridSize.y;

            do
            {
                i++;
                if (i > maxRetry)
                {
                    throw new Exception($"{nameof(GetEmptyPosition)} fail after {i} tries");
                }

                position = new Vector2Int(Random.Range(0, gridSize.x), Random.Range(0, gridSize.y));
            } while (!IsPositionEmpty(position));

            return position;

            bool IsPositionEmpty(Vector2Int position)
            {
                try
                {
                    IUnit unit = UnitGrid[position.x, position.y];
                    return unit == null;
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    return false;
                }
            }
        }

        internal Vector2Int GetBoardMiddle() => new Vector2Int(gridSize.x / 2, gridSize.y / 2);

        internal void AddUnit(IUnit unit)
        {
            Vector2Int position = unit.Position;
            if (UnitGrid[position.x, position.y] != null)
                throw new Exception($"This position is already occupied {position}");
            UnitGrid[position.x, position.y] = unit;
        }

        internal IUnit GetUnit(Vector2Int position) => UnitGrid[position.x, position.y];

        internal void Move(Vector2Int from, Vector2Int to)
        {
            IUnit sourceUnit = UnitGrid[from.x, from.y] ?? throw new Exception($"No unit at position {from}");
            IUnit targetUnit = UnitGrid[to.x, to.y];
            if (targetUnit != null)
                throw new Exception($"Position is occupied {to} by {targetUnit}");
            UnitGrid[to.x, to.y] = UnitGrid[from.x, from.y];
            UnitGrid[from.x, from.y] = null;

            if (UnitGrid[to.x, to.y] == null)
                throw new Exception("Reference got lost while move");

            UnitGrid[to.x, to.y].Position = to;

            if (UnitGrid[from.x, from.y] != null)
                throw new Exception("Source not get replace with null");
        }

        [Serializable]
        private class TestUnit : IUnit
        {
            [SerializeField] private int health;
            [SerializeField] private int attack;
            [SerializeField] private int defense;
            [SerializeField] private Vector2Int position;
            public int Health { get => health; set => health = value; }
            public int Attack { get => attack; set => attack = value; }
            public int Defense { get => defense; set => defense = value; }
            public Vector2Int Position { get => position; set => position = value; }
        }
    }
}