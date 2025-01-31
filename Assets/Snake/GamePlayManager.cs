using Snake.Movement;
using Snake.Player;
using Snake.Unit;
using Snake.World;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

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


        protected override void Start()
        {
            base.Start();

            InitilizeGame();
        }


        public void InitilizeGame()
        {
            worldGrid.CreateGrid(worldGridSize);

            SnakePlayer snakePlayer = SpawnPlayer();
            _SpawnUnitType(UnitType.HERO);
            _SpawnUnitType(UnitType.MONSTER);


            void _SpawnUnitType(UnitType unitType)
            {
                GameSetting.SpawnSetting spawnSetting = gameSetting.GetSpawnSetting(unitType);
                for (int i = 0; i < spawnSetting.maxSpawnCount; i++)
                {
                    IUnit unit = SpawnUnitType(unitType, worldGrid.GetEmptyPosition());
                    worldGrid.AddUnit(unit);
                }
            }

            SnakePlayer SpawnPlayer()
            {
                SnakePlayer snakePlayer = Instantiate(spawnableReference.playerRef, worldGrid.transform, false);
                snakePlayer.transform.position = worldGrid.transform.position;
                Vector2Int spawnPosition = worldGrid.GetBoardMiddle();
                IUnit firstChildHero = SpawnUnitType(UnitType.HERO, spawnPosition);
                snakePlayer.ChildHero.Add(firstChildHero);
                worldGrid.AddUnit(firstChildHero);
                snakePlayer.Position = spawnPosition;
                SnakeMovement snakeMovement = snakePlayer.GetComponent<SnakeMovement>();
                snakeMovement.onMove = (movementContext) => OnPlayerMove(snakePlayer, movementContext);

                //temp just for clarification
                snakePlayer.GetComponent<Renderer>().material.color = Color.blue;

                return snakePlayer;
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
                    break;
                case IMonster monster:
                    //TODO: Combat
                    //TODO: Move the player into position
                    //TODO: Remove monster from the map
                    Debug.LogWarning("Collide with monster");
                    break;
                case IPlayer player:
                    Debug.LogWarning("Collide with player");
                    break;
                case null:
                    Debug.LogWarning("No Collision, just move");
                    break;
                default:
                    break;
            }
            //TODO: Move child hero follow player
            //TODO: Pass direction of child to the next

            MoveSnakePlayer(playerUnit, nextPosition);

            return true;

            ///Move entire snake player and its tails (hero line)
            void MoveSnakePlayer(IPlayer playerUnit, Vector2Int nextPosition)
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
                            direction = playerUnit.Direction;
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
        }
    }
}