using System;
using System.Collections.Generic;
using System.Linq;
using SpringChallenge2022.Actions;
using SpringChallenge2022.Common.Services;
using SpringChallenge2022.Models;

namespace SpringChallenge2022.Agents
{
    internal class BronzeBoss
    {
        private readonly HashSet<int> _targetedMonsterIds = new HashSet<int>();

        public IReadOnlyList<IAction> GetAction(Game game)
        {
            var actions = new Dictionary<int, IAction>();

            var rankedMonsters = GetRankedMonsters(game);

            foreach (var (_, hero) in game.MyHeroes)
            {
                if (hero.TargetedMonster != null)
                {
                    if (!game.Monsters.ContainsKey(hero.TargetedMonster.Id))
                    {
                        Io.Debug($"removing {hero.TargetedMonster.Id} for {hero.Id}");
                        _targetedMonsterIds.Remove(hero.TargetedMonster.Id);
                        hero.TargetedMonster = null;
                    }
                    else
                    {
                        var monster = game.Monsters[hero.TargetedMonster.Id];
                        Io.Debug($"hero {hero.Id} already targeted {monster.Id}");
                        actions.Add(hero.Id, new MoveAction(monster.Position));
                        continue;
                    }
                }

                Monster bestMonster = null;
                var bestMonsterDistance = int.MaxValue;

                foreach (var (_, monster) in rankedMonsters)
                {
                    if (_targetedMonsterIds.Contains(monster.Id))
                    {
                        Io.Debug($"Monster {monster.Id} already targeted");
                        continue;
                    }

                    var distance = hero.Position.GetDistanceSquared(monster.Position);

                    if (distance < bestMonsterDistance)
                    {
                        bestMonsterDistance = distance;
                        bestMonster = monster;
                    }
                }

                if (bestMonster != null)
                {
                    _targetedMonsterIds.Add(bestMonster.Id);
                    hero.TargetedMonster = bestMonster;
                    Io.Debug($"hero {hero.Id} targeting {hero.TargetedMonster.Id}");
                    actions.Add(hero.Id, new MoveAction(bestMonster.Position));
                }
                else
                {
                    if (rankedMonsters.Any())
                    {
                        actions.Add(hero.Id, new MoveAction(rankedMonsters[0].Item2.Position));
                    }
                    else
                    {
                        actions.Add(hero.Id, new MoveAction(hero.StartingPosition));
                    }

                    // actions.Add(new MoveAction(new Vector(Constants.RandomGenerator.Next(5000), Constants.RandomGenerator.Next(5000))));
                }
            }

            return actions.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        }

        private IReadOnlyList<Tuple<int, Monster>> GetRankedMonsters(Game game)
        {
            var rankedMonsters = new List<Tuple<int, Monster>>();

            foreach (var (_, monster) in game.Monsters)
            {
                if (monster.ThreatFor == 1)
                {
                    var threatLevel = 0;
                    var distance = game.MyBase.Position.GetDistanceSquared(monster.Position);
                    var distanceScore = 500 * (1 / distance + 1);

                    if (monster.TargetingBase)
                    {
                        threatLevel = 1000 + distanceScore;
                    }
                    else
                    {
                        threatLevel = 500 + distanceScore;
                    }

                    rankedMonsters.Add(new Tuple<int, Monster>(threatLevel, monster));
                }

                rankedMonsters = rankedMonsters.OrderByDescending(x => x.Item1).ToList();
            }

            return rankedMonsters;
        }
    }
}
