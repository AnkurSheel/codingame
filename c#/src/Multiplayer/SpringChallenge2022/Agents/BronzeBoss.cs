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
        public IReadOnlyList<IAction> GetAction(Game game)
        {
            var actions = new Dictionary<int, IAction>();

            var rankedMonsters = GetRankedMonsters(game);

            Io.Debug($"RankedMonsters {rankedMonsters.Count}");

            foreach (var monster in rankedMonsters)
            {
                Io.Debug($"Evaluating {monster.Id}");

                var heroTargetingMonster = game.MyPlayer.Heroes.Values.SingleOrDefault(x => x.TargetedMonster?.Id == monster.Id);

                if (heroTargetingMonster != null)
                {
                    Io.Debug($"hero {heroTargetingMonster.Id} already targeted {monster.Id}");
                    actions.Add(heroTargetingMonster.Id, new MoveAction(monster.Position));
                    continue;
                }

                Hero bestHero = null;
                var bestHeroDistance = int.MaxValue;

                foreach (var hero in game.MyPlayer.Heroes.Values)
                {
                    if (hero.TargetedMonster != null)
                    {
                        continue;
                    }

                    var distance = (int)(hero.Position - monster.Position).LengthSquared();

                    if (distance < bestHeroDistance)
                    {
                        bestHeroDistance = distance;
                        bestHero = hero;
                    }
                }

                if (bestHero != null)
                {
                    bestHero.TargetedMonster = monster;
                    Io.Debug($"hero {bestHero.Id} targeting {bestHero.TargetedMonster.Id}");
                    actions.Add(bestHero.Id, new MoveAction(monster.Position));
                }
            }

            foreach (var hero in game.MyPlayer.Heroes.Values)
            {
                if (!actions.ContainsKey(hero.Id))
                {
                    if (rankedMonsters.Any())
                    {
                        actions.Add(hero.Id, new MoveAction(rankedMonsters[0].Position));
                    }
                    else
                    {
                        actions.Add(hero.Id, new MoveAction(hero.StartingPosition));
                    }
                }
            }

            return actions.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        }

        private IReadOnlyList<Monster> GetRankedMonsters(Game game)
        {
            var rankedMonsters = new List<Tuple<int, Monster>>();

            foreach (var (_, monster) in game.Monsters)
            {
                if (monster.ThreatFor == 1)
                {
                    var threatLevel = 0;
                    var distance = (int)(game.MyPlayer.BasePosition - monster.Position).LengthSquared();
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
            }

            return rankedMonsters.OrderByDescending(x => x.Item1).Select(x => x.Item2).ToList();
        }
    }
}
