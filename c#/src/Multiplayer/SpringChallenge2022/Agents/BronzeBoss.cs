using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SpringChallenge2022.Actions;
using SpringChallenge2022.Common.Services;
using SpringChallenge2022.Models;

namespace SpringChallenge2022.Agents
{
    internal class BronzeBoss
    {
        private Dictionary<int, IAction> _actions;
        private int _availableMana;
        private Game _game;

        public IReadOnlyList<IAction> GetAction(Game game)
        {
            _game = game;
            _actions = new Dictionary<int, IAction>();
            _availableMana = game.MyPlayer.Mana;

            var rankedMonsters = GetRankedMonsters();

            if (rankedMonsters.Count > Constants.NumberOfHeroes)
            {
                AddSpellActionIfTooManyMonsters(rankedMonsters);
            }

            Io.Debug($"RankedMonsters {rankedMonsters.Count}");

            foreach (var rankedMonster in rankedMonsters)
            {
                Io.Debug($"Evaluating {rankedMonster}");

                var heroTargetingMonster = game.MyPlayer.Heroes.Values.SingleOrDefault(x => x.TargetedMonster?.Id == rankedMonster.Monster.Id);

                if (heroTargetingMonster != null)
                {
                    var action = GetActionIfHeroAlreadyTargetingMonster(heroTargetingMonster, rankedMonster);
                    _actions.Add(heroTargetingMonster.Id, action);
                }
                else
                {
                    var hero = GetHeroToTargetMonster(rankedMonster);

                    if (hero != null)
                    {
                        hero.TargetedMonster = rankedMonster.Monster;
                        Io.Debug($"hero {hero.Id} targeting {hero.TargetedMonster.Id}");
                        _actions.Add(hero.Id, new MoveAction(rankedMonster.Monster.Position));
                    }
                }
            }

            AddActionsForHeroesWithoutAction(rankedMonsters.Select(x => x.Monster).ToList());

            return _actions.OrderBy(x => x.Key).Select(x => x.Value).ToList();
        }

        private IReadOnlyList<RankedMonster> GetRankedMonsters()
        {
            var rankedMonsters = new List<RankedMonster>();

            foreach (var (_, monster) in _game.Monsters)
            {
                if (monster.ThreatFor == 1)
                {
                    var turnsToReach = monster.GetTurnsToReach(_game.MyPlayer.BasePosition);
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

        private IAction GetActionIfHeroAlreadyTargetingMonster(Hero hero, RankedMonster rankedMonster)
        {
            Io.Debug($"hero {hero.Id} already targeted {rankedMonster.Monster.Id}");

            var spellAction = GetSpellAction(hero, rankedMonster);

            return spellAction ?? new MoveAction(rankedMonster.Monster.Position);
        }

        private IAction? GetSpellAction(Hero hero, RankedMonster rankedMonster)
        {
            if (CanCastWindSpell(hero, rankedMonster))
            {
                return GetSpellAction(SpellType.Wind, null);
            }
            else if (CanCastControlSpell(hero, rankedMonster.Monster))
            {
                var distance = (rankedMonster.Monster.Position - _game.MyPlayer.BasePosition).Length();

                if (distance > Constants.BaseRadius)
                {
                    return GetSpellAction(SpellType.Control, rankedMonster.Monster);
                }
            }

            return null;
        }

        private IAction GetSpellAction(SpellType spellType, Monster? monster)
        {
            _availableMana -= Constants.ManaRequiredForSpell;
            return spellType switch
            {
                SpellType.Wind => new WindSpellAction(_game.OpponentPlayer.BasePosition),
                SpellType.Control => new ControlSpellAction(monster.Id, _game.OpponentPlayer.BasePosition),
                SpellType.Shield => throw new ArgumentOutOfRangeException(nameof(spellType), spellType, null),
                _ => throw new ArgumentOutOfRangeException(nameof(spellType), spellType, null)
            };
        }

        private bool CanCastWindSpell(Hero hero, RankedMonster rankedMonster)
            => _availableMana >= Constants.ManaRequiredForSpell && rankedMonster.TurnsToReach <= rankedMonster.ShotsNeeded && IsMonsterInRange(hero, rankedMonster.Monster, Constants.WindSpellRange);

        private bool CanCastControlSpell(Hero hero, Monster rankedMonster)
            => _availableMana >= Constants.ManaRequiredForSpell && IsMonsterInRange(hero, rankedMonster, Constants.ControlSpellRange);

        private Hero GetHeroToTargetMonster(RankedMonster rankedMonster)
        {
            Hero bestHero = null;
            var bestHeroDistance = int.MaxValue;

            foreach (var hero in _game.MyPlayer.Heroes.Values)
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

        private void AddActionsForHeroesWithoutAction(IReadOnlyList<Monster> rankedMonsters)
        {
            foreach (var hero in _game.MyPlayer.Heroes.Values)
            {
                if (!_actions.ContainsKey(hero.Id))
                {
                    var monstersForWildMana = _game.Monsters.Values.Where(monster => IsMonsterValidForWildMana(_game.MyPlayer.BasePosition, monster)).ToList();

                    var action = GetActionIfDoingNothing(hero, rankedMonsters, monstersForWildMana);

                    _actions.Add(hero.Id, action);
                }
            }
        }

        private MoveAction GetActionIfDoingNothing(Hero hero, IReadOnlyList<Monster> rankedMonsters, IReadOnlyList<Monster> monstersForWildMana)
        {
            Monster? bestMonster;

            if (monstersForWildMana.Any())
            {
                bestMonster = GetClosestMonster(hero, monstersForWildMana);

                if (bestMonster != null)
                {
                    Io.Debug($"Moving hero {hero.Id} to Monster {bestMonster.Id} for Wild Mana");
                    return new MoveAction(bestMonster.Position);
                }
            }

            // bestMonster = GetClosestMonster(hero, rankedMonsters);
            //
            // if (bestMonster != null)
            // {
            //     return new MoveAction(bestMonster.Position);
            // }

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

        private void AddSpellActionIfTooManyMonsters(IReadOnlyList<RankedMonster> rankedMonsters)
        {
            foreach (var hero in _game.MyPlayer.Heroes.Values)
            {
                if (rankedMonsters.Any(rankedMonster => CanCastWindSpell(hero, rankedMonster)))
                {
                    Io.Debug($"removing {hero.TargetedMonster?.Id} from {hero.Id} because of wind action");
                    hero.TargetedMonster = null;
                    _actions.Add(hero.Id, GetSpellAction(SpellType.Wind, null));
                    break;
                }
            }
        }

        private bool IsMonsterValidForWildMana(Vector2 basePosition, Monster monster)
        {
            var distance = (monster.Position - basePosition).Length();
            Io.Debug($"Monster Id {monster.Id} :  Distance {distance}");
            return distance > Constants.BaseRadius && distance < Constants.MaxDistanceFromBaseForHero;
        }
    }
}
