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
                    var action = GetActionIfHeroAlreadyTargetingMonster(game, heroTargetingMonster, rankedMonster);
                    actions.Add(heroTargetingMonster.Id, action);
                }
                else
                {
                    var hero = GetHeroToTargetMonster(game, rankedMonster);

                    if (hero != null)
                    {
                        hero.TargetedMonster = rankedMonster.Monster;
                        Io.Debug($"hero {hero.Id} targeting {hero.TargetedMonster.Id}");
                        actions.Add(hero.Id, new MoveAction(rankedMonster.Monster.Position));
                    }
                }
            }

            GetActionIfDoingNothing(game, rankedMonsters, actions);

            return actions.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        }

        private IReadOnlyList<RankedMonster> GetRankedMonsters(Game game)
        {
            var rankedMonsters = new List<RankedMonster>();

            foreach (var (_, monster) in game.Monsters)
            {
                if (monster.ThreatFor == 1)
                {
                    var turnsToReach = monster.GetTurnsToReach(game.MyPlayer.BasePosition);
                    var shotsNeeded = monster.GetHitsNeeded();

                    var distanceScore = Constants.DistanceBaseScore / (turnsToReach + 1);
                    var shotsNeededScore = shotsNeeded * Constants.ShotsNeededBaseScore;

                    var baseThreatLevel = monster.TargetingBase
                        ? Constants.TargetingBaseBaseScore
                        : Constants.NonTargetingBaseBaseScore;

                    var threatLevel = baseThreatLevel + distanceScore + shotsNeededScore;

                    Io.Debug($"Scores : threatLevel {threatLevel} : baseThreatLevel {baseThreatLevel} : distanceScore {distanceScore} : shotsNeededScore {shotsNeededScore}");

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

        private bool IsMonsterInRange(Hero hero, Monster monster, int range)
        {
            var distance = (hero.Position - monster.Position).Length();
            return distance < range;
        }

        private IAction GetActionIfHeroAlreadyTargetingMonster(Game game, Hero hero, RankedMonster rankedMonster)
        {
            Io.Debug($"hero {hero.Id} already targeted {rankedMonster.Monster.Id}");

            var spellAction = GetSpellAction(game, hero, rankedMonster);

            return spellAction ?? new MoveAction(rankedMonster.Monster.Position);
        }

        private IAction? GetSpellAction(Game game, Hero hero, RankedMonster rankedMonster)
        {
            if (game.MyPlayer.Mana < Constants.ManaRequiredForSpell)
            {
                return null;
            }

            if (rankedMonster.TurnsToReach <= rankedMonster.ShotsNeeded && IsMonsterInRange(hero, rankedMonster.Monster, Constants.WindSpellRange))
            {
                return new WindSpellAction(game.OpponentPlayer.BasePosition);
            }
            // else
            // {
            //     return IsMonsterInRange(hero, rankedMonster.Monster, Constants.ControlSpellRange)
            //         ? new ControlSpellAction(rankedMonster.Monster.Id, game.OpponentPlayer.BasePosition)
            //         : null;
            // }

            return null;
        }

        private Hero GetHeroToTargetMonster(Game game, RankedMonster rankedMonster)
        {
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

            return bestHero;
        }

        private void GetActionIfDoingNothing(Game game, IReadOnlyList<RankedMonster> rankedMonsters, Dictionary<int, IAction> actions)
        {
            foreach (var hero in game.MyPlayer.Heroes.Values)
            {
                if (!actions.ContainsKey(hero.Id))
                {
                    var bestMonster = GetClosestMonster(hero, rankedMonsters);

                    var action = bestMonster != null
                        ? new MoveAction(bestMonster.Position)
                        : new MoveAction(hero.StartingPosition);

                    actions.Add(hero.Id, action);
                }
            }
        }

        private static Monster? GetClosestMonster(Hero hero, IReadOnlyList<RankedMonster> rankedMonsters)
        {
            Monster bestMonster = null;
            var bestMonsterDistance = int.MaxValue;

            foreach (var rankedMonster in rankedMonsters)
            {
                var distance = (int)(hero.Position - rankedMonster.Monster.Position).LengthSquared();

                if (distance < bestMonsterDistance)
                {
                    bestMonsterDistance = distance;
                    bestMonster = rankedMonster.Monster;
                }
            }

            return bestMonster;
        }
    }
}
