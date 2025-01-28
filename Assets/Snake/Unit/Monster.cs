using Snake.Unit;
using UnityEngine;

namespace Snake
{
    public class Monster : CustomMonoBehaviour, IMonster
    {
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public Vector2Int Position { get; set; }
    }
}
