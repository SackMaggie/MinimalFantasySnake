

using Snake.Movement;
using Snake.Player;
using Snake.Unit;
using Snake.World;
using System;
using System.Collections.Generic;
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
            worldGrid.AddUnit(snakePlayer);
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
                snakePlayer.Position = worldGrid.GetBoardMiddle();
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

            IUnit playerUnit = snakePlayer;
            Vector2Int currentPosition = snakePlayer.Position;
            IUnit unitOnPosition = worldGrid.GetUnit(currentPosition);
            if (unitOnPosition == null || playerUnit != unitOnPosition)
                throw new Exception($"Player is not on world grid");

            //process movementContext
            Vector2Int nextPosition = movementContext.newDirection.GetRelativePosition(currentPosition);
            IUnit unit = worldGrid.GetUnit(nextPosition);
            switch (unit)
            {
                case IHeros hero:
                    Debug.LogWarning("Collide with hero");
                    worldGrid.Move(currentPosition, nextPosition);
                    break;
                case IMonster monster:
                    Debug.LogWarning("Collide with monster");
                    worldGrid.Move(currentPosition, nextPosition);
                    break;
                case IPlayer player:
                    Debug.LogWarning("Collide with player");
                    worldGrid.Move(currentPosition, nextPosition);
                    break;
                case null:
                    Debug.LogWarning("No Collision, just move");
                    worldGrid.Move(currentPosition, nextPosition);
                    break;
                default:
                    break;
            }

            return true;
        }
    }
}