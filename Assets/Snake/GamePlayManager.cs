using Snake.Battle;
using Snake.Movement;
using Snake.Player;
using Snake.Unit;
using Snake.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Snake
{
    public class GamePlayManager : CustomMonoBehaviour
    {
        public WorldGrid worldGrid;
        private Vector2Int worldGridSize;
        [Space]
        public SpawnableReference spawnableReference;
        public GameSetting gameSetting;
        public BattleManager battleManager;
        public Arena arena;
        public UnityEvent<IUnit> OnUnitSpawn = new UnityEvent<IUnit>();
        public UnityEvent<IUnit> OnUnitKill = new UnityEvent<IUnit>();
        public UnityEvent<GameState> OnGameStateChange = new UnityEvent<GameState>();
        private GameState gameState = GameState.None;
        public int CurrentUnitId { get; private set; } = 1;
        public SnakePlayer SnakePlayer { get; private set; }

        public GameState GameState
        {
            get => gameState;
            set
            {
                Debug.Log($"GameState {value}");
                gameState = value;
                GameStateList.Add(value);
                OnGameStateChange.Invoke(value);
            }
        }

        public List<GameState> GameStateList = new List<GameState>();

        public void InitilizeGame()
        {
            GameStateList.Clear();
            if (GameState == GameState.GameEnded)
                GameState = GameState.CleanUp;
            GameState = GameState.Initilizing;
            foreach (IUnit item in GetAllUnit())
            {
                item.OnKilled.RemoveListener(OnUnitKilled);
                if (item.GameObject != null)
                    Destroy(item.GameObject);
            }
            worldGridSize = gameSetting.GetBoardSize();
            arena.Init(worldGridSize);
            worldGrid.CreateGrid(worldGridSize);
            battleManager = new BattleManager
            {
                gamePlayManager = this
            };
            if (SnakePlayer != null)
                Destroy(SnakePlayer.gameObject);
            SnakePlayer = SpawnPlayer();
            TintTheObject(SnakePlayer.CurrentHero);
            _SpawnUnitType(UnitType.HERO);
            _SpawnUnitType(UnitType.MONSTER);
            _SpawnUnitType(UnitType.ITEM);
            _SpawnUnitType(UnitType.OBSTACLE);
            GameState = GameState.InitilizeFinish;

            GameState = GameState.Playing;


            void _SpawnUnitType(UnitType unitType)
            {
                GameSetting.SpawnSetting spawnSetting = gameSetting.GetSpawnSetting(unitType);
                for (int i = 0; i < spawnSetting.maxSpawnCount; i++)
                {
                    IUnit unit = SpawnUnitType(unitType, worldGrid.GetEmptyPosition());
                }
            }

            SnakePlayer SpawnPlayer()
            {
                SnakePlayer snakePlayer = Instantiate(spawnableReference.playerRef, worldGrid.transform, false);
                snakePlayer.transform.position = worldGrid.transform.position;
                Vector2Int spawnPosition = worldGrid.GetBoardMiddle();
                IUnit firstChildHero = SpawnUnitType(UnitType.HERO, spawnPosition);
                snakePlayer.ChildHero.Add(firstChildHero);
                snakePlayer.Position = spawnPosition;
                SnakeMovement snakeMovement = snakePlayer.GetComponent<SnakeMovement>();
                snakeMovement.CheckCanMoveFunc = CheckCanPlayerMove;
                snakeMovement.RequestMovementFunc = (movementContext) => OnPlayerMove(snakePlayer, movementContext);
                snakeMovement.RequestCycleUnitAction = OnPressCycleUnit;
                return snakePlayer;
            }
        }

        private void OnUnitKilled((IUnit unit, IUnit killer) arg)
        {
            IUnit unit = arg.unit;
            IUnit killer = arg.killer;
            //Only in play state
            if (GameState != GameState.Playing)
                return;
            Debug.Log($"OnUnitKilled {unit} by {killer}");
            OnUnitKill.Invoke(unit);

            if (SnakePlayer.ChildHero.Count == 0)
            {
                GameState = GameState.GameEnded;
            }

            if (unit is IUnitObstacle obstacle)
            {
                // an Obstacle is a unit cover multiple spots
                foreach (Vector2Int item in obstacle.SubPosition)
                    RemoveUnitAtPosition(unit, item);
            }
            RemoveUnitAtPosition(unit, unit.Position);
            UnitType unitType = unit.GetUnitType();
            ReSpawnUnitFromChance(unitType);
            EnsureMinimumNumberOfUnitType(unitType);

            void RemoveUnitAtPosition(IUnit unit, Vector2Int position)
            {
                IUnit unit1 = worldGrid.GetUnit(position);
                if (unit1 != null)
                {
                    if (unit1 == unit)
                        worldGrid.Remove(unit.Position);
                    else
                        Debug.LogError($"Requested Unit are not the same as unit in world grid, hash {unit.GetHashCode()} and {unit1.GetHashCode()} {unit} {unit1}");
                }
            }
        }

        private void OnHeroRecruited(IHeros hero)
        {
            ReSpawnUnitFromChance(UnitType.HERO);
            EnsureMinimumNumberOfUnitType(UnitType.HERO);
        }

        private void ReSpawnUnitFromChance(UnitType unitType)
        {
            GameSetting.SpawnSetting spawnSetting = gameSetting.GetSpawnSetting(unitType);
            // spawn same unit type based on configured chance
            if (Random.value <= spawnSetting.spawnChance)
                SpawnUnitType(unitType, worldGrid.GetEmptyPosition());
        }

        private void EnsureMinimumNumberOfUnitType(UnitType unitType)
        {
            GameSetting.SpawnSetting spawnSetting = gameSetting.GetSpawnSetting(unitType);
            IEnumerable<IUnit> query = from IUnit unit in worldGrid.UnitGrid
                                       where unit != null && unit.GetUnitType() == unitType && !SnakePlayer.ChildHero.Contains(unit)
                                       select unit;
            int v = query.Count();
            Debug.Log($"EnsureMinimumNumberOfUnitType {unitType} {v}");
            for (int i = 0; i < spawnSetting.minSpawnCount - v; i++)
            {
                SpawnUnitType(unitType, worldGrid.GetEmptyPosition());
            }
        }

        private bool CheckCanPlayerMove()
        {
            switch (GameState)
            {
                case GameState.Playing:
                    //todo add combat check if adding non instant combat


                    return true;
                default:
                    throw new Exception($"Invalid GameState {GameState}");
            }
        }

        private IUnit SpawnUnitType(UnitType unitType)
        {
            GameObject gameObjectRef;
            GameObject newGameObject;
            IUnit unit;
            switch (unitType)
            {
                case UnitType.HERO:
                case UnitType.MONSTER:
                case UnitType.OBSTACLE:
                    gameObjectRef = spawnableReference.GetObjectFromType(unitType);
                    newGameObject = Instantiate(gameObjectRef, worldGrid.transform, false);
                    newGameObject.transform.position = worldGrid.transform.position;
                    unit = newGameObject.GetComponent<IUnit>();
                    break;
                case UnitType.ITEM:
                    GameSetting.ItemBinding itemBinding = gameSetting.GetRandomItem();
                    gameObjectRef = itemBinding.objectBinding;
                    newGameObject = Instantiate(gameObjectRef, worldGrid.transform, false);
                    newGameObject.transform.position = worldGrid.transform.position;
                    IItem item = newGameObject.GetComponent<IItem>();
                    unit = item;

                    item.ApplyItemProperty(itemBinding.property);
                    break;
                default:
                    throw new NotImplementedException(unitType.ToString());
            }

            TintTheObject(unit);

            return unit;
        }

        /// <summary>
        /// temp just for clarification
        /// </summary>
        /// <param name="unit"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void TintTheObject(IUnit unit)
        {
            if (unit == null)
                return;
            GameObject newGameObject = unit.GameObject;
            UnitType unitType = unit.GetUnitType();
            newGameObject.GetComponentInChildren<Renderer>().material.color = unitType switch
            {
                UnitType.MONSTER => Color.red,
                UnitType.HERO => SnakePlayer != null && SnakePlayer.ChildHero.Contains(unit) ? Color.blue : Color.green,
                UnitType.ITEM => Color.yellow,
                UnitType.OBSTACLE => Color.black,
                _ => throw new NotImplementedException(unitType.ToString()),
            };
        }

        private IUnit SpawnUnitType(UnitType unitType, Vector2Int position)
        {
            Debug.Log($"Spawn {unitType} at {position}");
            IUnit unit = SpawnUnitType(unitType);
            unit.UnitId = CurrentUnitId++;
            unit.Position = position;
            unit.Direction = GetRandomDirection();
            unit.UnitClass = GetRandomUnitClass();
            unit.ApplyStats(gameSetting.GetStatsSetting(unitType, unit.UnitClass));
            unit.OnKilled.AddListener(OnUnitKilled);
            worldGrid.AddUnit(unit);
            if (unit is IUnitObstacle obstacle)
                ApplyObstacleSetting(obstacle);
            OnUnitSpawn.Invoke(unit);
            return unit;

            static Direction GetRandomDirection()
            {
                // could be cached
                Direction[] directions = Enum.GetValues(typeof(Direction)).Cast<Direction>().ToArray();

                return directions[Random.Range(0, directions.Length)];
            }

            static UnitClassEnum GetRandomUnitClass()
            {
                // could be cached
                UnitClassEnum[] unitClasses = Enum.GetValues(typeof(UnitClassEnum)).Cast<UnitClassEnum>().Where(x => x != UnitClassEnum.None).ToArray();

                return unitClasses[Random.Range(0, unitClasses.Length)];
            }

            void ApplyObstacleSetting(IUnitObstacle obstacle)
            {
                Direction[] horizontals = new Direction[] { Direction.LEFT, Direction.RIGHT };
                Direction[] verticles = new Direction[] { Direction.UP, Direction.DOWN };

                Vector2Int size = Vector2Int.one;
                bool isFitToGrid = true;
                ///For fail safe in do-while loop
                int k = 0;
                int maxRetry = 20;
                Direction horizontalDirection = Direction.LEFT;
                Direction verticleDirection = Direction.UP;
                do
                {
                    k++;
                    if (k > maxRetry)
                        break;
                    size = gameSetting.GetRandomObstaclesSize();
                    horizontalDirection = horizontals[Random.Range(0, horizontals.Length)];
                    verticleDirection = verticles[Random.Range(0, horizontals.Length)];
                    //TestDirection(position, size.x, ref isFitToGrid, horizontalDirection);
                    //TestDirection(position, size.y, ref isFitToGrid, verticleDirection);
                    TestDirection(position, ref size, ref isFitToGrid, horizontalDirection, verticleDirection);
                } while (!isFitToGrid);
                if (!isFitToGrid)
                {
                    size = Vector2Int.one;
                    obstacle.SubPosition.Clear();
                }
                obstacle.HorizontalDirection = horizontalDirection;
                obstacle.VerticleDirection = verticleDirection;
                obstacle.Size = size;
                foreach (Vector2Int item in obstacle.SubPosition)
                    worldGrid.AddUnit(obstacle, item);

                void TestDirection(Vector2Int position, ref Vector2Int sizeS, ref bool isFitToGrid, Direction direction, Direction direction2)
                {
                    try
                    {
                        for (int x = 0; x < sizeS.x; x++)
                        {
                            for (int y = 0; y < sizeS.y; y++)
                            {
                                Vector2Int relativeX = direction.GetRelativePosition(position, x);
                                Vector2Int relativePos = direction2.GetRelativePosition(relativeX, y);
                                if (relativePos == position)
                                    continue;

                                IUnit unit1 = worldGrid.GetUnit(relativePos);
                                if (unit1 != null)
                                {
                                    Debug.Log($"occupied {relativePos} by {unit1}");
                                    isFitToGrid = false;
                                    break;
                                }
                                obstacle.SubPosition.Add(relativePos);
                            }
                            if (!isFitToGrid)
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        isFitToGrid = false;
                    }
                }

                void TestDirection2(Vector2Int position, int size, ref bool isFitToGrid, Direction direction)
                {
                    try
                    {
                        for (int i = 1; i < size; i++)
                        {
                            Vector2Int relativePos = direction.GetRelativePosition(position, i);
                            IUnit unit1 = worldGrid.GetUnit(relativePos);
                            if (unit1 != null)
                            {
                                Debug.Log($"occupied {relativePos} by {unit1}");
                                isFitToGrid = false;
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log(e);
                        isFitToGrid = false;
                    }
                }

            }
        }

        /// <summary>
        /// Check current position
        /// Check out of bound
        /// Check if collide with other stuff
        /// 
        /// </summary>
        /// <param name="snakePlayer"></param>
        /// <param name="movementContext"></param>
        /// <returns>
        /// true for successful move and false for any other reason
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool OnPlayerMove(IPlayer snakePlayer, MovementContext movementContext)
        {
            if (!CheckCanPlayerMove())
                return false;
            Debug.Log(movementContext.rawInput);
            //validation
            if (snakePlayer == null)
                throw new ArgumentNullException(nameof(snakePlayer));
            if (movementContext == null)
                throw new ArgumentNullException(nameof(movementContext));

            IPlayer playerUnit = snakePlayer;
            Vector2Int currentPosition = snakePlayer.Position;
            IUnit unitOnPosition = worldGrid.GetUnit(currentPosition);
            if (unitOnPosition == null)
                throw new Exception($"unit on the world grid is not exist {currentPosition}");
            if (playerUnit.CurrentHero != unitOnPosition)
                throw new Exception("current hero is not the same on world grid");

            playerUnit.Direction = movementContext.newDirection;

            //process movementContext
            Vector2Int nextPosition = movementContext.newDirection.GetRelativePosition(currentPosition);
            IUnit unit = worldGrid.GetUnit(nextPosition);
            return OnUnitCollision(playerUnit, nextPosition, unit);
        }

        private bool OnUnitCollision(IPlayer playerUnit, Vector2Int nextPosition, IUnit collisionUnit)
        {
            switch (collisionUnit)
            {
                case IHeros hero:
                    Debug.LogWarning("Collide with hero");
                    if (playerUnit.ChildHero.Contains(hero))
                    {
                        Debug.LogError("Game End");
                        GameState = GameState.GameEnded;
                        return false;
                    }
                    AddChildHero(playerUnit, nextPosition, hero, HeroAddMode.AddToBack);
                    OnHeroRecruited(hero);
                    TintTheObject(hero);
                    return true;
                case IMonster monster:
                    Debug.LogWarning("Collide with monster");
                    BattleResult battleResult = battleManager.Battle(playerUnit, monster);
                    switch (battleResult)
                    {
                        case BattleResult.Victory:
                            MoveSnakePlayer(playerUnit, nextPosition);
                            return true;
                        case BattleResult.Lose:
                        case BattleResult.Injured:
                            return false;
                        case BattleResult.None:
                            Debug.LogError($"Battle result in {battleResult} someting went wrong");
                            return false;
                        default:
                            throw new NotImplementedException(battleResult.ToString());
                    }
                case IPlayer player:
                    throw new InvalidOperationException($"Should not collide with {player}");
                case IItem item:
                    {
                        item.OnPickUp(playerUnit);
                        item.IsDead = true;
                        item.KillUnit(playerUnit);

                        IUnit currentHero = playerUnit.CurrentHero;
                        if (currentHero.Health <= 0)
                        {
                            // hero is dead swap the next hero to battle and restart the loop
                            Vector2Int position = currentHero.Position;
                            playerUnit.ChildHero.Remove(currentHero);
                            currentHero.IsDead = true;
                            currentHero.KillUnit(item);

                            if (playerUnit.ChildHero.Count > 0)
                                MoveSnakePlayer(playerUnit, position);
                        }
                        MoveSnakePlayer(playerUnit, nextPosition);
                        return true;
                    }
                case IUnitObstacle unitObstacle:
                    {
                        IUnit currentHero = playerUnit.CurrentHero;
                        currentHero.Health = 0;
                        if (currentHero.Health <= 0)
                        {
                            // hero is dead swap the next hero to battle and restart the loop
                            Vector2Int position = currentHero.Position;
                            playerUnit.ChildHero.Remove(currentHero);
                            currentHero.IsDead = true;
                            currentHero.KillUnit(unitObstacle);

                            if (playerUnit.ChildHero.Count > 0)
                                MoveSnakePlayer(playerUnit, position);
                        }
                        MoveSnakePlayer(playerUnit, nextPosition);
                        return true;
                    }
                case null:
                    Debug.LogWarning("No Collision, just move");
                    MoveSnakePlayer(playerUnit, nextPosition);
                    return true;
                default:
                    throw new NotImplementedException(collisionUnit.GetType().ToString());
            }
        }

        /// <summary>
        /// Rewrote and make it in-place swap
        /// TargetUnit get remove temporary move the rest of the line
        /// Add TargetUnit back to the line
        /// </summary>
        /// <param name="playerUnit"></param>
        /// <param name="nextPosition"></param>
        /// <param name="unit"></param>
        /// <param name="heroAddMode"></param>
        private void AddChildHero(IPlayer playerUnit, Vector2Int nextPosition, IUnit unit, HeroAddMode heroAddMode)
        {
            try
            {
                // target unit is use for location reference where the unit will be
                IUnit targetUnit = heroAddMode switch
                {
                    HeroAddMode.AddToBack => playerUnit.ChildHero.LastOrDefault(),
                    HeroAddMode.AddToFront => playerUnit.ChildHero.FirstOrDefault(),
                    _ => throw new NotImplementedException(heroAddMode.ToString()),
                };

                // temporary remove unit from the grid to allow the hero line move safely
                worldGrid.Remove(nextPosition);

                // change the unit position to where it should
                unit.Position = targetUnit.Position;
                unit.Direction = targetUnit.Direction;

                bool isMoveForward = heroAddMode switch
                {
                    HeroAddMode.AddToBack => true,
                    HeroAddMode.AddToFront => false,
                    _ => throw new NotImplementedException(),
                };

                // move head and hero line
                MoveSnakePlayer(playerUnit, nextPosition, isMoveForward);

                // add child to hero line
                switch (heroAddMode)
                {
                    case HeroAddMode.AddToBack:
                        playerUnit.ChildHero.Add(unit);
                        break;
                    case HeroAddMode.AddToFront:
                        playerUnit.ChildHero.Insert(0, unit);
                        break;
                    default:
                        throw new NotImplementedException(heroAddMode.ToString());
                }

                // re-added unit
                worldGrid.AddUnit(unit);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                throw;
            }
        }

        internal void MoveSnakePlayer(IPlayer playerUnit, Vector2Int nextPosition, bool moveForward = true)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Move Snake from {playerUnit.Position} to {nextPosition} direction {playerUnit.Direction}");
            try
            {
                IList<IUnit> childHero = moveForward ? playerUnit.ChildHero : playerUnit.ChildHero.Reverse().ToList();
                for (int i = childHero.Count - 1; i >= 1; i--)
                {
                    IUnit currentUnit = childHero[i];
                    currentUnit.Direction = childHero[i - 1].Direction;
                }
                Vector2Int lastPosition = Vector2Int.zero;
                for (int i = 0; i < childHero.Count; i++)
                {
                    Vector2Int targetPosition;
                    IUnit lastUnit;
                    IUnit currentUnit = childHero[i];
                    Direction direction;
                    if (i == 0)
                    {
                        targetPosition = nextPosition;
                        lastPosition = currentUnit.Position;
                        worldGrid.Move(currentUnit.Position, targetPosition);
                    }
                    else
                    {
                        lastUnit = childHero[i - 1];
                        targetPosition = lastPosition;
                        direction = lastUnit.Direction;
                        stringBuilder.AppendLine($"lastUnit {lastUnit.Position} {lastUnit.Direction}" +
                            $" self {currentUnit.Position} {currentUnit.Direction}" +
                            $" target position {targetPosition}");
                        lastPosition = currentUnit.Position;
                        worldGrid.Move(currentUnit.Position, targetPosition);
                    }
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                Debug.Log(stringBuilder.ToString());
            }
        }

        internal IUnit[] GetAllUnit()
        {
            if (worldGrid == null || worldGrid.UnitGrid == null)
                return Array.Empty<IUnit>();
            IEnumerable<IUnit> query = from IUnit item in worldGrid.UnitGrid
                                       where item != null
                                       select item;
            return query.ToArray();
        }

        private void OnPressCycleUnit(bool isForward)
        {
            if (GameState != GameState.Playing)
                return;
            // no hero to cycle
            if (SnakePlayer.ChildHero.Count < 2)
                return;
            IUnit unit = isForward ? SnakePlayer.ChildHero.First() : SnakePlayer.ChildHero.Last();
            Vector2Int position = unit.Position;
            SnakePlayer.ChildHero.Remove(unit);
            AddChildHero(SnakePlayer, position, unit, isForward ? HeroAddMode.AddToBack : HeroAddMode.AddToFront);
        }
    }

    public enum GameState
    {
        None,
        Initilizing,
        InitilizeFinish,
        Playing,
        GameEnded,
        CleanUp,
    }

    public enum HeroAddMode
    {
        AddToBack,
        AddToFront,
    }
}