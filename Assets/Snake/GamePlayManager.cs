﻿using Snake.Battle;
using Snake.Movement;
using Snake.Player;
using Snake.Unit;
using Snake.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Snake
{
    public class GamePlayManager : CustomMonoBehaviour
    {
        ///TODO: Create A UI
        ///TODO: Make a map grid size of 16x16 <see cref="World.WorldGrid"/>
        ///TODO: Make a unit
        ///Hero <see cref="Snake.Unit.IHeros"/>
        ///Player <see cref="Snake.Unit.IPlayer"/> <see cref="Player.SnakePlayer"/>
        ///Monster <see cref="Snake.Unit.IMonster"/>
        ///TODO: Turn based control
        ///TODO: Collision Combat


        public WorldGrid worldGrid;
        private Vector2Int worldGridSize = new Vector2Int(16, 16);
        [Space]
        public SpawnableReference spawnableReference;
        public GameSetting gameSetting;
        public BattleManager battleManager;
        private SnakePlayer snakePlayer;
        public UnityEvent<IUnit> OnUnitSpawn = new UnityEvent<IUnit>();
        public UnityEvent<IUnit> OnUnitKill = new UnityEvent<IUnit>();
        public UnityEvent<GameState> OnGameStateChange = new UnityEvent<GameState>();
        private GameState gameState = GameState.None;

        public GameState GameState
        {
            get => gameState;
            set
            {
                gameState = value;
                OnGameStateChange.Invoke(value);
            }
        }

        public void InitilizeGame()
        {
            GameState = GameState.Initilizing;
            foreach (IUnit item in GetAllUnit())
            {
                if (item.GameObject != null)
                    Destroy(item.GameObject);
            }
            worldGrid.CreateGrid(worldGridSize);
            battleManager = new BattleManager
            {
                gamePlayManager = this
            };
            if (snakePlayer != null)
                snakePlayer.KillUnit(null);
            snakePlayer = SpawnPlayer();
            _SpawnUnitType(UnitType.HERO);
            _SpawnUnitType(UnitType.MONSTER);
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
                return snakePlayer;
            }
        }

        private void OnUnitKilled((IUnit unit, IUnit killer) arg)
        {
            IUnit unit = arg.unit;
            IUnit killer = arg.killer;
            Debug.Log($"OnUnitKilled {unit} {killer}");
            OnUnitKill.Invoke(unit);

            if (snakePlayer.ChildHero.Count == 0)
            {
                GameState = GameState.GameEnded;
            }

            IUnit unit1 = worldGrid.GetUnit(unit.Position);
            if (unit1 == unit)
                worldGrid.Remove(unit.Position);
            else
                Debug.LogError("Requested Unit are not the same as unit in world grid");
            UnitType unitType = unit.GetUnitType();
            GameSetting.SpawnSetting spawnSetting = gameSetting.GetSpawnSetting(unitType);
            // spawn same unit type based on configured chance
            if (Random.value <= spawnSetting.spawnChance)
                SpawnUnitType(unitType, worldGrid.GetEmptyPosition());

            EnsureMinimumNumberOfUnitType(unitType);
        }

        private void EnsureMinimumNumberOfUnitType(UnitType unitType)
        {
            GameSetting.SpawnSetting spawnSetting = gameSetting.GetSpawnSetting(unitType);
            IEnumerable<IUnit> query = from IUnit unit in worldGrid.UnitGrid
                                       where unit.GetUnitType() == unitType
                                       select unit;
            for (int i = 0; i < spawnSetting.minSpawnCount; i++)
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
            GameObject gameObjectRef = spawnableReference.GetObjectFromType(unitType);
            GameObject newGameObject = Instantiate(gameObjectRef, worldGrid.transform, false);
            newGameObject.transform.position = worldGrid.transform.position;

            //temp just for clarification
            newGameObject.GetComponent<Renderer>().material.color = unitType switch
            {
                UnitType.MONSTER => Color.red,
                UnitType.HERO => Color.green,
                _ => throw new NotImplementedException(unitType.ToString()),
            };

            return newGameObject.GetComponent<IUnit>();
        }

        private IUnit SpawnUnitType(UnitType unitType, Vector2Int position)
        {
            IUnit unit = SpawnUnitType(unitType);
            unit.Position = position;
            unit.ApplyStats(gameSetting.GetStatsSetting(unitType));
            unit.OnKilled.AddListener(OnUnitKilled);
            worldGrid.AddUnit(unit);
            OnUnitSpawn.Invoke(unit);
            return unit;
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
        private bool OnPlayerMove(SnakePlayer snakePlayer, MovementContext movementContext)
        {
            if (!CheckCanPlayerMove())
                return false;
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
            switch (unit)
            {
                case IHeros hero:
                    Debug.LogWarning("Collide with hero");
                    if (playerUnit.ChildHero.Contains(hero))
                    {
                        Debug.LogError("Game End");
                        GameState = GameState.GameEnded;
                        return false;
                    }
                    //TODO: Check if that hero is our child for gameover
                    //TODO: Remove hero from the map
                    //TODO: Move the player into position
                    //TODO: Add hero as a player child
                    //TODO: Add hero back to the map
                    IUnit lastHeroUnit = playerUnit.ChildHero.LastOrDefault();

                    //Use direction from last unit or player
                    Direction direction = lastHeroUnit != null ? lastHeroUnit.Direction : playerUnit.Direction;
                    Vector2Int lastChildHeroPosition = lastHeroUnit != null ? lastHeroUnit.Position : playerUnit.Position;

                    //move hero to the back of the line
                    Debug.Log($"Last Hero Unit {direction} {lastChildHeroPosition}", lastHeroUnit.GameObject);
                    worldGrid.Move(nextPosition, direction.GetRelativePosition(lastChildHeroPosition, -1));
                    hero.Direction = direction;

                    playerUnit.ChildHero.Add(hero);
                    MoveSnakePlayer(playerUnit, nextPosition);
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
                case null:
                    Debug.LogWarning("No Collision, just move");
                    MoveSnakePlayer(playerUnit, nextPosition);
                    return true;
                default:
                    throw new NotImplementedException(unit.GetType().ToString());
            }
        }

        internal void MoveSnakePlayer(IPlayer playerUnit, Vector2Int nextPosition)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Move Snake from {playerUnit.Position} to {nextPosition} direction {playerUnit.Direction}");
            try
            {
                Vector2Int lastPosition = Vector2Int.zero;
                for (int i = 0; i < playerUnit.ChildHero.Count; i++)
                {
                    Vector2Int targetPosition;
                    IUnit lastUnit;
                    IUnit currentUnit = playerUnit.ChildHero[i];
                    Direction direction;
                    if (i == 0)
                    {
                        targetPosition = nextPosition;
                        lastPosition = currentUnit.Position;
                        worldGrid.Move(currentUnit.Position, targetPosition);
                    }
                    else
                    {
                        lastUnit = playerUnit.ChildHero[i - 1];
                        targetPosition = lastPosition;
                        direction = lastUnit.Direction;
                        stringBuilder.AppendLine($"lastUnit {lastUnit.Position} {lastUnit.Direction}" +
                            $" self {currentUnit.Position} {currentUnit.Direction}" +
                            $" target position {targetPosition}");
                        lastPosition = currentUnit.Position;
                        worldGrid.Move(currentUnit.Position, targetPosition);
                        currentUnit.Direction = direction;
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
    }

    public enum GameState
    {
        None,
        Initilizing,
        Playing,
        GameEnded,
    }
}