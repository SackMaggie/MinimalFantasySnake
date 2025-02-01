using Snake.Movement;
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

#if UNITY_EDITOR
        protected override void Update()
        {
            base.Update();

            //TODO: Remove this, for debugging only
            if (UnitGrid != null)
            {
                units.Clear();
                foreach (IUnit item in UnitGrid)
                {
                    if (item is IUnit unit)
                    {
                        units.Add(new TestUnit()
                        {
                            position = unit.Position,
                            attack = unit.Attack,
                            defense = unit.Defense,
                            health = unit.Health,
                            gameObject = unit.GameObject
                        });
                    }
                }
            }
        }
#endif

        internal void CreateGrid(Vector2Int gridSize)
        {
            this.gridSize = gridSize;
            UnitGrid = new IUnit[gridSize.x, gridSize.y];
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
            AddUnit(unit, position);
        }

        internal void AddUnit(IUnit unit, Vector2Int position)
        {
            if (UnitGrid[position.x, position.y] != null)
                throw new Exception($"This position is already occupied {position}");
            UnitGrid[position.x, position.y] = unit;
        }

        internal IUnit GetUnit(Vector2Int position) => UnitGrid[position.x, position.y];

        /// <summary>
        /// Use this function to move unit to empty position
        /// Will throw an error if unit is not exist or the location is occupied
        /// It is similar to <see cref="Swap(Vector2Int, Vector2Int)"/> but it has different intention
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="Exception"></exception>
        internal void Move(Vector2Int from, Vector2Int to)
        {
            try
            {
                if (from == to)
                    throw new Exception($"same position {from} : {to} unable to move");
                IUnit sourceUnit = UnitGrid[from.x, from.y] ?? throw new Exception($"No unit at position {from}");
                IUnit targetUnit = UnitGrid[to.x, to.y];
                if (targetUnit != null && !targetUnit.IsDead)
                    throw new Exception($"Position is occupied {to} by {targetUnit}");
                UnitGrid[to.x, to.y] = UnitGrid[from.x, from.y];
                UnitGrid[from.x, from.y] = null;

                if (UnitGrid[to.x, to.y] == null)
                    throw new Exception("Reference got lost while move");

                UnitGrid[to.x, to.y].Position = to;

                if (UnitGrid[from.x, from.y] != null)
                    throw new Exception("Source not get replace with null");
            }
            catch (Exception e)
            {
                throw new Exception($"Error when Move is called {from}>>{to}", e);
            }
        }

        /// <summary>
        /// Use this function to swap both unit position
        /// Will throw an error if ther's no unit to swap
        /// It is similar to <see cref="Move(Vector2Int, Vector2Int)"/> but it has different intention
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <exception cref="Exception"></exception>
        internal void Swap(Vector2Int from, Vector2Int to)
        {
            IUnit sourceUnit = UnitGrid[from.x, from.y] ?? throw new Exception($"No unit at position {from}");
            IUnit targetUnit = UnitGrid[to.x, to.y] ?? throw new Exception($"No unit at position {to}");

            UnitGrid[to.x, to.y] = sourceUnit;
            UnitGrid[from.x, from.y] = targetUnit;

            if (UnitGrid[to.x, to.y] == null)
                throw new Exception("Reference got lost while swap");
            if (UnitGrid[from.x, from.y] != null)
                throw new Exception("Source not get replace with null");
        }

        internal void Remove(Vector2Int position)
        {
            UnitGrid[position.x, position.y] = null;
        }

        [Serializable]
        private class TestUnit
        {
            public int health;
            public int attack;
            public int defense;
            public Vector2Int position;
            public Direction direction;
            public GameObject gameObject;
        }
    }
}