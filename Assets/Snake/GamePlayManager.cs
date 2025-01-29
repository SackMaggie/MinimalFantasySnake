

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

            SpawnPlayer();
            _SpawnUnitType(UnitType.HERO);
            _SpawnUnitType(UnitType.MONSTER);


            void _SpawnUnitType(UnitType unitType)
            {
                GameSetting.SpawnSetting spawnSetting = gameSetting.GetSpawnSetting(unitType);
                for (int i = 0; i < spawnSetting.maxSpawnCount; i++)
                {
                    SpawnUnitType(unitType, worldGrid.GetEmptyPosition());
                }
            }

            SnakePlayer SpawnPlayer()
            {
                SnakePlayer snakePlayer = Instantiate(spawnableReference.playerRef, worldGrid.transform, false);
                snakePlayer.Position = worldGrid.GetBoardMiddle();
                SnakeMovement snakeMovement = snakePlayer.GetComponent<SnakeMovement>();
                snakeMovement.onMove = (movementContext) => OnPlayerMove(snakePlayer, movementContext);
                return snakePlayer;
            }
        }

        private IUnit SpawnUnitType(UnitType unitType)
        {
            GameObject gameObjectRef = spawnableReference.GetObjectFromType(unitType);
            GameObject gameObject = Instantiate(gameObjectRef, worldGrid.transform, false);
            gameObject.transform.position = worldGrid.transform.position;
            return gameObject.GetComponent<IUnit>();
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
            throw new NotImplementedException();
        }
    }
}