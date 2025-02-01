using Snake.Unit;
using System;
using System.Text;
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
        public GamePlayManager gamePlayManager;

        /// <summary>
        /// Call this when battle occur, It is basically fight till death
        /// if the monster is still alive use the current hero to fight
        /// if the hero is dead, send the next hero in line to fight
        /// keep doing it untill either monster is dead or depleated hero line
        /// </summary>
        /// <param name="player"></param>
        /// <param name="monster"></param>
        public BattleResult Battle(IPlayer player, IMonster monster)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("battle start");
            try
            {
                if (player is null)
                    throw new ArgumentNullException(nameof(player));

                if (monster is null)
                    throw new ArgumentNullException(nameof(monster));

                if (monster is IPlayer or IHeros)
                    throw new Exception("No combat between player or hero allowed, hero get recruited, player is just game over");
                BattleResult battleResult = BattleResult.None;

                ///For fail safe in do-while loop
                int i = 0;
                int maxRetry = Mathf.Min(player.ChildHero.Count, 1) * 10;
                do
                {
                    i++;
                    stringBuilder.AppendLine($"loop {i}");
                    if (i > maxRetry)
                    {
                        throw new Exception($"{nameof(Battle)} fail after {i} tries");
                    }
                    IUnit currentHero = player.CurrentHero;
                    if (currentHero == null)
                    {
                        battleResult = BattleResult.Lose;
                        throw new Exception("No active hero, probally no Hero left");
                    }
                    int damageToMonster = CalulateDamage(currentHero, monster);
                    int damageToHero = CalulateDamage(monster, currentHero);
                    stringBuilder.AppendLine($"monster stats {monster.Health} {monster.Attack} {monster.Defense}");
                    stringBuilder.AppendLine($"hero stats {currentHero.Health} {currentHero.Attack} {currentHero.Defense}");
                    stringBuilder.AppendLine($"damageToMonster {damageToMonster} damageToHero {damageToHero}");
                    stringBuilder.AppendLine($"before monster.Health {monster.Health} currentHero.Health {currentHero.Health}");
                    monster.Health -= damageToMonster;
                    currentHero.Health -= damageToHero;
                    stringBuilder.AppendLine($"after monster.Health {monster.Health} currentHero.Health {currentHero.Health}");

                    if (monster.Health <= 0)
                    {
                        monster.IsDead = true;
                        monster.KillUnit(currentHero);
                        battleResult = BattleResult.Victory;
                    }
                    if (currentHero.Health <= 0)
                    {
                        // hero is dead swap the next hero to battle and restart the loop
                        Vector2Int position = currentHero.Position;
                        player.ChildHero.Remove(currentHero);
                        currentHero.IsDead = true;
                        currentHero.KillUnit(monster);

                        if (player.ChildHero.Count > 0)
                            gamePlayManager.MoveSnakePlayer(player, position);
                        battleResult = BattleResult.Lose;
                        continue;
                    }
                    // break and exit the loop since battle is finished
                    break;
                } while (monster != null && monster.Health > 0);

                if (monster != null && monster.Health > 0)
                    battleResult = BattleResult.Injured;

                return battleResult;
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

        private static int CalulateDamage(IUnit attacker, IUnit defender)
        {
            //formula: Damage = (Attacker Attack - Defender Defense)
            //attack and defense should not be negative
            //damage output should not be negative
            return attacker.Attack < 0
                ? throw new ArgumentException($"Unit have negative attack")
                : defender.Defense < 0
                ? throw new ArgumentException($"Unit have negative defense")
                : Mathf.Max(attacker.Attack - defender.Defense, 0);
        }
    }

    public enum BattleResult
    {
        None,
        Lose,
        Victory,
        Injured,
    }
}