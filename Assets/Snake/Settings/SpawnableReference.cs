using Snake.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Snake
{
    [CreateAssetMenu(menuName = "Snake")]
    public class SpawnableReference : ScriptableObject
    {
        public List<SpawnableRef> spawnables = new List<SpawnableRef>();
        public SnakePlayer playerRef;

        internal GameObject GetObjectFromType(UnitType unitType)
        {
            return spawnables.FirstOrDefault(x => x.unitType == unitType)?.gameObject;
        }

        [Serializable]
        public class SpawnableRef
        {
            public UnitType unitType;
            public GameObject gameObject;
        }
    }
}