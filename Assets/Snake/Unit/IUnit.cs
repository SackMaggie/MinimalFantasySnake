using Snake.Movement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Snake.Unit
{
    public interface IUnit : IUnitStat
    {
        int UnitId { get; set; }
        Vector2Int Position { get; set; }
        Direction Direction { get; set; }
        GameObject GameObject { get; }
        bool IsDead { get; set; }

        void KillUnit(IUnit killer);

        UnityEvent<(IUnit unit, IUnit killer)> OnKilled { get; }
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

    public interface IUnitStat
    {
        int Health { get; set; }
        int Attack { get; set; }
        int Defense { get; set; }
        UnitClassEnum UnitClass { get; set; }
    }

    public interface IUnitObstacle : IUnit
    {
        Vector2Int Size { get; set; }
        Direction HorizontalDirection { get; set; }
        Direction VerticleDirection { get; set; }
        IList<Vector2Int> SubPosition { get; }
    }
}
