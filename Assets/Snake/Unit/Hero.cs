using Snake.Unit;
using UnityEngine;

namespace Snake
{
    public class Hero : CustomMonoBehaviour, IHeros
    {
        public int Health { get; set; }
        public int Attack { get; set; }
        public int Defense { get; set; }
        public Vector2Int Position { get; set; }
    }
}
