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

            foreach (var rankedMonster in rankedMonsters)
            {
                Io.Debug($"Evaluating {rankedMonster}");

                var heroTargetingMonster = game.MyPlayer.Heroes.Values.SingleOrDefault(x => x.TargetedMonster?.Id == rankedMonster.Monster.Id);

                if (heroTargetingMonster != null)
                {
                    Io.Debug($"hero {heroTargetingMonster.Id} already targeted {rankedMonster.Monster.Id}");

                    if (rankedMonster.TurnsToReach <= rankedMonster.ShotsNeeded
                        && game.MyPlayer.Mana > Constants.ManaRequiredForSpell
                        && IsMonsterInRange(heroTargetingMonster, rankedMonster.Monster, Constants.ControlSpellRange))
                    {
                        actions.Add(heroTargetingMonster.Id, new ControlSpellAction(rankedMonster.Monster.Id, game.OpponentPlayer.BasePosition));
                    }
                    else
                    {
                        actions.Add(heroTargetingMonster.Id, new MoveAction(rankedMonster.Monster.Position));
                    }

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

                    var distance = (int)(hero.Position - rankedMonster.Monster.Position).LengthSquared();

                    if (distance < bestHeroDistance)
                    {
                        bestHeroDistance = distance;
                        bestHero = hero;
                    }
                }

                if (bestHero != null)
                {
                    bestHero.TargetedMonster = rankedMonster.Monster;
                    Io.Debug($"hero {bestHero.Id} targeting {bestHero.TargetedMonster.Id}");
                    actions.Add(bestHero.Id, new MoveAction(rankedMonster.Monster.Position));
                }
            }

            foreach (var hero in game.MyPlayer.Heroes.Values)
            {
                if (!actions.ContainsKey(hero.Id))
                {
                    if (rankedMonsters.Any())
                    {
                        actions.Add(hero.Id, new MoveAction(rankedMonsters[0].Monster.Position));
                    }
                    else
                    {
                        actions.Add(hero.Id, new MoveAction(hero.StartingPosition));
                    }
                }
            }

            return actions.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        }

        private bool IsMonsterInRange(Hero hero, Monster monster, int range)
        {
            var distance = (hero.Position - monster.Position).Length();
            return distance < range;
        }

        private IReadOnlyList<RankedMonster> GetRankedMonsters(Game game)
        {
            var rankedMonsters = new List<RankedMonster>();

            foreach (var (_, monster) in game.Monsters)
            {
                if (monster.ThreatFor == 1)
                {
                    var threatLevel = 0.0f;

                    var turnsToReach = monster.GetTurnsToReach(game.MyPlayer.BasePosition);
                    var shotsNeeded = monster.GetHitsNeeded();

                    var distanceScore = 500.0f * (1.0f / (turnsToReach + 1));

                    Io.Debug($"{distanceScore}");
                    if (monster.TargetingBase)
                    {
                        threatLevel = 1000 + distanceScore;
                    }
                    else
                    {
                        threatLevel = 500 + distanceScore;
                    }

                    rankedMonsters.Add(
                        new RankedMonster(
                            monster,
                            threatLevel,
                            turnsToReach,
                            shotsNeeded));
                }
            }

            return rankedMonsters.OrderByDescending(x => x.ThreatLevel).ToList();
        }
    }
}
