using System.Collections.Generic;
using System.Linq;
using SpringChallenge2022.Actions;
using SpringChallenge2022.Common.Services;
using SpringChallenge2022.Models;

namespace SpringChallenge2022.Agents
{
    internal class BronzeBoss
    {
        private Dictionary<int, IAction> _actions;

        public IReadOnlyList<IAction> GetAction(Game game)
        {
            _actions = new Dictionary<int, IAction>();

            var rankedMonsters = GetRankedMonsters(game);

            if (rankedMonsters.Count > Constants.NumberOfHeroes)
            {
                AddSpellActionIfTooManyMonsters(game, rankedMonsters);
            }

            Io.Debug($"RankedMonsters {rankedMonsters.Count}");

            foreach (var rankedMonster in rankedMonsters)
            {
                Io.Debug($"Evaluating {rankedMonster}");

                var heroTargetingMonster = game.MyPlayer.Heroes.Values.SingleOrDefault(x => x.TargetedMonster?.Id == rankedMonster.Monster.Id);

                if (heroTargetingMonster != null)
                {
                    var action = GetActionIfHeroAlreadyTargetingMonster(game, heroTargetingMonster, rankedMonster);
                    _actions.Add(heroTargetingMonster.Id, action);
                }
                else
                {
                    var hero = GetHeroToTargetMonster(game, rankedMonster);

                    if (hero != null)
                    {
                        hero.TargetedMonster = rankedMonster.Monster;
                        Io.Debug($"hero {hero.Id} targeting {hero.TargetedMonster.Id}");
                        _actions.Add(hero.Id, new MoveAction(rankedMonster.Monster.Position));
                    }
                }
            }

            AddActionsForHeroesWithoutAction(game, rankedMonsters.Select(x => x.Monster).ToList());

            return _actions.OrderBy(x => x.Key).Select(x => x.Value).ToList();
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
            if (CanCastWindSpell(game, hero, rankedMonster))
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

        private bool CanCastWindSpell(Game game, Hero hero, RankedMonster rankedMonster)
            => game.MyPlayer.Mana >= Constants.ManaRequiredForSpell
               && rankedMonster.TurnsToReach <= rankedMonster.ShotsNeeded
               && IsMonsterInRange(hero, rankedMonster.Monster, Constants.WindSpellRange);

        private Hero GetHeroToTargetMonster(Game game, RankedMonster rankedMonster)
        {
            Hero bestHero = null;
            var bestHeroDistance = int.MaxValue;

            foreach (var hero in game.MyPlayer.Heroes.Values)
            {
                if (_actions.ContainsKey(hero.Id))
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

        private void AddActionsForHeroesWithoutAction(Game game, IReadOnlyList<Monster> monsters)
        {
            foreach (var hero in game.MyPlayer.Heroes.Values)
            {
                if (!_actions.ContainsKey(hero.Id))
                {
                    var action = GetActionIfDoingNothing(game, monsters, hero);

                    _actions.Add(hero.Id, action);
                }
            }
        }

        private static MoveAction GetActionIfDoingNothing(Game game, IReadOnlyList<Monster> monsters, Hero hero)
        {
            var bestMonster = GetClosestMonster(hero, monsters);

            if (bestMonster != null)
            {
                return new MoveAction(bestMonster.Position);
            }

            if (game.Monsters.Any())
            {
                bestMonster = GetClosestMonster(hero, game.Monsters.Values.ToList());

                if (bestMonster != null)
                {
                    return new MoveAction(game.Monsters.First().Value.Position);
                }
            }

            return new MoveAction(hero.StartingPosition);
        }

        private static Monster? GetClosestMonster(Hero hero, IReadOnlyList<Monster> monsters)
        {
            Monster bestMonster = null;
            var bestMonsterDistance = int.MaxValue;

            foreach (var monster in monsters)
            {
                var distance = (int)(hero.Position - monster.Position).LengthSquared();

                if (distance < bestMonsterDistance)
                {
                    bestMonsterDistance = distance;
                    bestMonster = monster;
                }
            }

            return bestMonster;
        }

        private void AddSpellActionIfTooManyMonsters(Game game, IReadOnlyList<RankedMonster> rankedMonsters)
        {
            foreach (var hero in game.MyPlayer.Heroes.Values)
            {
                if (rankedMonsters.Any(rankedMonster => CanCastWindSpell(game, hero, rankedMonster)))
                {
                    Io.Debug($"removing {hero.TargetedMonster?.Id} from {hero.Id} because of wind action");
                    hero.TargetedMonster = null;
                    _actions.Add(hero.Id, new WindSpellAction(game.OpponentPlayer.BasePosition));
                    break;
                }
            }
        }
    }
}
