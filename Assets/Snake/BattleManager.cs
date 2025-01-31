using Snake.Unit;
using System;
using UnityEngine;

namespace Snake.Battle
{
    /// <summary>
    /// - [ ] When a battle occurs, reduce health both Hero and Monster at the same time using this formula: Damage = (Attacker Attack - Defender Defense)
    /// - [ ] If the Health of the Monster is 0 or lower, remove that enemy from the game and spawn new Monsters according to the chance configuration.
    /// - [ ] If the Health of the Hero is 0 or lower, remove that hero from the game and move the rest of the line normally.
    /// </summary>
    public class BattleManager
    {
        /// <summary>
        /// Call this when battle occur, It is basically fight till death
        /// if the monster is still alive use the current hero to fight
        /// if the hero is dead, send the next hero in line to fight
        /// keep doing it untill either monster is dead or depleated hero line
        /// </summary>
        /// <param name="player"></param>
        /// <param name="monster"></param>
        public static void Battle(IPlayer player, IMonster monster)
        {
            if (player is null)
                throw new ArgumentNullException(nameof(player));

            if (monster is null)
                throw new ArgumentNullException(nameof(monster));

            if (monster is IPlayer or IHeros)
                throw new Exception("No combat between player or hero allowed, hero get recruited, player is just game over");
            ///For fail safe in do-while loop
            int i = 0;
            int maxRetry = Mathf.Min(player.ChildHero.Count, 1) * 10;
            do
            {
                i++;
                if (i > maxRetry)
                {
                    throw new Exception($"{nameof(Battle)} fail after {i} tries");
                }
                IUnit currentHero = player.CurrentHero;
                if (currentHero == null)
                    throw new Exception("No active hero, probally no Hero left");
                int damageToMonster = CalulateDamage(currentHero, monster);
                int damageToHero = CalulateDamage(monster, currentHero);
                Debug.Log($"monster stats {monster.Health} {monster.Attack} {monster.Defense}");
                Debug.Log($"hero stats {currentHero.Health} {currentHero.Attack} {currentHero.Defense}");
                Debug.Log($"damageToMonster {damageToMonster} damageToHero {damageToHero}");
                Debug.Log($"before monster.Health {monster.Health} currentHero.Health {currentHero.Health}");
                monster.Health -= damageToMonster;
                currentHero.Health -= damageToHero;
                Debug.Log($"after monster.Health {monster.Health} currentHero.Health {currentHero.Health}");

                if (monster.Health <= 0)
                    monster.KillUnit(currentHero);
                if (currentHero.Health <= 0)
                {
                    // hero is dead swap the next hero to battle and restart the loop
                    player.ChildHero.Remove(currentHero);
                    currentHero.KillUnit(monster);
                    continue;
                }
                // break and exit the loop since battle is finished
                break;
            } while (monster != null && monster.Health > 0);
        }

        private static int CalulateDamage(IUnit attacker, IUnit defender)
        {
            //formula: Damage = (Attacker Attack - Defender Defense)
            //attack and defense should not be negative
            //damage output should not be negative
            if (attacker.Attack < 0)
                throw new ArgumentException($"Unit have negative attack");
            if (defender.Defense < 0)
                throw new ArgumentException($"Unit have negative defense");
            return Mathf.Max(attacker.Attack - defender.Defense, 0);
        }
    }
}