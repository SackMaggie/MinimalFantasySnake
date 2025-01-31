using Snake.Movement;
using System.Collections.Generic;
using UnityEngine;

namespace Snake.Unit
{
    public interface IUnit
    {
        int Health { get; set; }
        int Attack { get; set; }
        int Defense { get; set; }
        Vector2Int Position { get; set; }
        Direction Direction { get; set; }
        GameObject GameObject { get; }
    }

    public interface IHeros : IUnit
    {
        
    }

    public interface IMonster : IUnit
    {

    }

    /// <summary>
    /// Can recruit <see cref="IHeros"/>
    /// Combat with <see cref="IMonster"/>
    /// Kill himself when touch
    /// </summary>
    public interface IPlayer : IUnit
    {
        IList<IUnit> ChildHero { get; }
        IUnit CurrentHero { get; set; }
    }
}
