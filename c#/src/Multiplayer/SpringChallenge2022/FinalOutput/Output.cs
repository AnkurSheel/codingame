using System;
using System.Numerics;
using System.Collections.Generic;
using System.Linq;
using SpringChallenge2022;
using SpringChallenge2022.Common.Services;
using SpringChallenge2022.Models;
using SpringChallenge2022.Agents;
using SpringChallenge2022.Actions;
using System.IO;


 // 25/04/2022 08:53


namespace SpringChallenge2022
{
    public static class Constants
    {
        public static readonly Random RandomGenerator = new Random(123);
        public static readonly Vector2 BottomRightMap = new Vector2(17630, 9000);
        public const int MonsterSpeed = 400;
        public const int HeroDamageDistance = 800;
        public const int MonsterBaseDistanceForDamage = 300;
        public const int DamagePerHit = 2;
        public const int ManaRequiredForSpell = 10;
        public const int ControlSpellRange = 2200;
        public const int WindSpellRange = 1280;
        public const int BaseRadius = 5000;
        public const int DistanceFromBaseForStartingPosition = BaseRadius + 1000;
        public const int MaxDistanceFromBaseForHero = BaseRadius + 2000;
        public const float DistanceBaseScore = 10000.0f;
        public const float ShotsNeededBaseScore = 20.0f;
        public const float TargetingBaseBaseScore = 1000.0f;
        public const float NonTargetingBaseBaseScore = 500.0f;
        public const int NumberOfHeroes = 3;

    }
}

internal class Game
{
    private int _heroesPerPlayer;

    public Player MyPlayer { get; private set; }

    public Player OpponentPlayer { get; private set; }

    public Dictionary<int, Monster> Monsters { get; private set; } = new Dictionary<int, Monster>();

    public void Initialize()
    {
        var inputs = Io.ReadLine().Split(' ');

        // base_x,base_y: The corner of the map representing your base
        var baseX = int.Parse(inputs[0]);
        var baseY = int.Parse(inputs[1]);

        MyPlayer = new Player(new Vector2(baseX, baseY));
        OpponentPlayer = new Player(new Vector2(Constants.BottomRightMap.X - baseX, Constants.BottomRightMap.Y - baseY));

        // heroesPerPlayer: Always 3
        _heroesPerPlayer = int.Parse(Io.ReadLine());
    }

    public void Reinit()
    {
        var inputs = Io.ReadLine().Split(' ');
        var myHealth = int.Parse(inputs[0]); // Your base health
        var myMana = int.Parse(inputs[1]); // Ignore in the first league; Spend ten mana to cast a spell

        inputs = Io.ReadLine().Split(' ');
        var oppHealth = int.Parse(inputs[0]);
        var oppMana = int.Parse(inputs[1]);

        ReInitEntities();

        MyPlayer.Update(myHealth, myMana, Monsters.Values.ToList());
        OpponentPlayer.Update(oppHealth, oppMana, Monsters.Values.ToList());
    }

    private void ReInitEntities()
    {
        foreach (var monster in Monsters.Values)
        {
            monster.Reset();
        }

        var entityCount = int.Parse(Io.ReadLine()); // Amount of heros and monsters you can see

        float heroAngleForStartingPosition = 15;

        for (var i = 0; i < entityCount; i++)
        {
            var inputs = Io.ReadLine().Split(' ');
            var id = int.Parse(inputs[0]); // Unique identifier
            var type = int.Parse(inputs[1]); // 0=monster, 1=your hero, 2=opponent hero
            var x = int.Parse(inputs[2]); // Position of this entity
            var y = int.Parse(inputs[3]);
            var shieldLife = int.Parse(inputs[4]); // Ignore for this league; Count down until shield spell fades
            var isControlled = int.Parse(inputs[5]); // Ignore for this league; Equals 1 when this entity is under a control spell
            var health = int.Parse(inputs[6]); // Remaining health of this monster
            var vx = int.Parse(inputs[7]); // Trajectory of this monster
            var vy = int.Parse(inputs[8]);
            var nearBase = int.Parse(inputs[9]); // 0=monster with no target yet, 1=monster targeting a base
            var threatFor = int.Parse(inputs[10]); // Given this monster's trajectory, is it a threat to 1=your base, 2=your opponent's base, 0=neither

            var position = new Vector2(x, y);

            switch (type)
            {
                case 0:
                    if (!Monsters.ContainsKey(id))
                    {
                        var monster = new Monster(
                            id,
                            position,
                            health,
                            new Vector2(vx, vy),
                            nearBase == 1,
                            threatFor);
                        Monsters.Add(id, monster);
                    }
                    else
                    {
                        Monsters[id]
                        .Update(
                            position,
                            health,
                            new Vector2(vx, vy),
                            nearBase == 1,
                            threatFor);
                    }

                    break;
                case 1:
                    if (MyPlayer.Heroes.ContainsKey(id))
                    {
                        MyPlayer.Heroes[id].Update(position);
                    }
                    else
                    {
                        var startingPosition = GetStartingPositionForHero(x, heroAngleForStartingPosition);
                        heroAngleForStartingPosition += 30;

                        MyPlayer.Heroes.Add(id, new Hero(id, position, startingPosition));
                    }

                    break;
                case 2:
                    if (OpponentPlayer.Heroes.ContainsKey(id))
                    {
                        OpponentPlayer.Heroes[id].Update(position);
                    }
                    else
                    {
                        OpponentPlayer.Heroes.Add(id, new Hero(id, position, position));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        Monsters = Monsters.Values.Where(x => x.Reinit).ToDictionary(v => v.Id, v => v);
    }

    private Vector2 GetStartingPositionForHero(int posX, float heroAngleForStartingPosition)
    {
        var angleForStartingPosition = posX > Constants.BottomRightMap.X / 2
            ? 270 - heroAngleForStartingPosition
            : heroAngleForStartingPosition;

        var direction = Vector2Extensions.GetDirection(angleForStartingPosition);

        Io.Debug($"Angle: {angleForStartingPosition}, Direction {direction} : Angle {direction.GetAngle()}");

        var startingPosition = MyPlayer.BasePosition + direction * Constants.DistanceFromBaseForStartingPosition;

        Io.Debug($"StartingPosition: {startingPosition}");
        return startingPosition;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        Io.Initialize();

        var game = new Game();
        var agent = new SilverBoss();

        game.Initialize();

        // game loop
        while (true)
        {
            game.Reinit();
            var actions = agent.GetAction(game);

            foreach (var action in actions)
            {
                Io.WriteLine(action.GetOutputAction());
            }
        }
    }
}

namespace SpringChallenge2022.Actions
{
    public class ControlSpellAction : IAction
    {
        private readonly int _id;
        private readonly Vector2 _targetPosition;

        public ControlSpellAction(int id, Vector2 targetPosition)
        {
            _id = id;
            _targetPosition = targetPosition;
        }

        public string GetOutputAction()
            => $"SPELL CONTROL {_id} {(int)_targetPosition.X} {(int)_targetPosition.Y}";

    }
}
namespace SpringChallenge2022.Actions
{
    public interface IAction
    {
        string GetOutputAction();
    }
}

namespace SpringChallenge2022.Actions
{
    internal class MoveAction : IAction
    {
        private readonly Vector2 _position;

        public MoveAction(Vector2 position)
        {
            _position = position;
        }

        public string GetOutputAction()
            => $"MOVE {(int)_position.X} {(int)_position.Y}";
    }
}
namespace SpringChallenge2022.Actions
{
    public class ShieldSpellAction : IAction
    {
        private readonly int _id;
        private const int Range = 2200;

        public ShieldSpellAction(int id)
        {
            _id = id;
        }

        public string GetOutputAction()
            => $"SPELL SHIELD {_id}";
    }
}
namespace SpringChallenge2022.Actions
{
    internal class WaitAction : IAction
    {
        public string GetOutputAction()
            => "WAIT";
    }
}

namespace SpringChallenge2022.Actions
{
    public class WindSpellAction : IAction
    {
        private const int Range = 1280;
        private readonly Vector2 _targetPosition;

        public WindSpellAction(Vector2 targetPosition)
        {
            _targetPosition = targetPosition;
        }

        public string GetOutputAction()
            => $"SPELL WIND {(int)_targetPosition.X} {(int)_targetPosition.Y}";
    }
}

namespace SpringChallenge2022.Agents
{
    internal class AgentWood1Boss
    {
        private readonly HashSet<int> _targetedMonsterIds = new HashSet<int>();

        public IReadOnlyList<IAction> GetAction(Game game)
        {
            var actions = new Dictionary<int, IAction>();

            var rankedMonsters = GetRankedMonsters(game);

            foreach (var hero in game.MyPlayer.Heroes.Values)
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

                    var distance = (int)(hero.Position - monster.Position).LengthSquared();

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

                rankedMonsters = rankedMonsters.OrderByDescending(x => x.Item1).ToList();
            }

            return rankedMonsters;
        }
    }
}

namespace SpringChallenge2022.Agents
{
    internal class AgentWood2Boss
    {
        public IReadOnlyList<IAction> GetAction(Game game)
        {
            var actions = new List<IAction>();

            var rankedMonsters = new List<Tuple<int, Monster>>();

            foreach (var (_, monster) in game.Monsters)
            {
                var threatLevel = 0;

                if (monster.TargetingBase && monster.ThreatFor == 1)
                {
                    threatLevel = 1000;
                }
                else if (monster.ThreatFor == 1)
                {
                    threatLevel = 500;
                }

                var distance = (int)(game.MyPlayer.BasePosition - monster.Position).LengthSquared();
                var distanceScore = 500 * (1 / distance + 1);

                rankedMonsters.Add(new Tuple<int, Monster>(threatLevel + distanceScore, monster));
            }

            rankedMonsters = rankedMonsters.OrderByDescending(x => x.Item1).ToList();

            for (var index = 0; index < game.MyPlayer.Heroes.Count; index++)
            {
                if (rankedMonsters.Count > index)
                {
                    var monster = rankedMonsters[index].Item2;
                    actions.Add(new MoveAction(monster.Position));
                }
                else
                {
                    actions.Add(new WaitAction());
                }
            }

            return actions;
        }
    }
}

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

            foreach (var monster in _game.Monsters.Values)
            {
                Io.Debug($"MonsterID: {monster.Id}");
            }

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

            foreach (var monster in _game.Monsters.Values)
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

        private bool IsMonsterInRange(
            Hero hero,
            Monster monster,
            int range,
            int minDistance)
        {
            var distance = (hero.Position - monster.Position).Length();
            return distance < range && distance > minDistance;
        }

        private IAction GetActionIfHeroAlreadyTargetingMonster(Hero hero, RankedMonster targetedMonster)
        {
            Io.Debug($"hero {hero.Id} already targeted {targetedMonster.Monster.Id}");

            var spellAction = GetSpellAction(hero, targetedMonster);

            return spellAction ?? new MoveAction(targetedMonster.Monster.Position);
        }

        private IAction? GetSpellAction(Hero hero, RankedMonster targetedMonster)
        {
            if (CanCastWindSpell(hero, targetedMonster))
            {
                return GetSpellAction(SpellType.Wind, null);
            }
            else if (CanCastControlSpell(hero, targetedMonster.Monster))
            {
                var distance = (targetedMonster.Monster.Position - _game.MyPlayer.BasePosition).Length();

                if (distance > Constants.BaseRadius)
                {
                    return GetSpellAction(SpellType.Control, targetedMonster.Monster);
                }
            }

            return null;
        }

        private IAction GetSpellAction(SpellType spellType, Monster? monster)
        {
            _availableMana -= Constants.ManaRequiredForSpell;

            switch (spellType)
            {
                case SpellType.Wind:
                    return new WindSpellAction(_game.OpponentPlayer.BasePosition);
                case SpellType.Control:
                    monster.SetControlled();
                    return new ControlSpellAction(monster.Id, _game.OpponentPlayer.BasePosition);
                case SpellType.Shield:
                default:
                    throw new ArgumentOutOfRangeException(nameof(spellType), spellType, null);
            }
        }

        private bool CanCastWindSpell(Hero hero, RankedMonster rankedMonster)
            => _availableMana >= Constants.ManaRequiredForSpell
            && rankedMonster.Monster.TargetingBase
            && IsMonsterInRange(
                hero,
                rankedMonster.Monster,
                Constants.WindSpellRange,
                0);

        private bool CanCastControlSpell(Hero hero, Monster rankedMonster)
            => _availableMana >= Constants.ManaRequiredForSpell
                && IsMonsterInRange(
                    hero,
                    rankedMonster,
                    Constants.ControlSpellRange,
                    Constants.HeroDamageDistance)
                && !rankedMonster.ControlledByMe;

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
            var monstersForWildMana = _game.Monsters.Values.Where(monster => !monster.ControlledByMe && IsMonsterValidForWildMana(monster)).ToList();

            foreach (var hero in _game.MyPlayer.Heroes.Values)
            {
                if (!_actions.ContainsKey(hero.Id))
                {
                    var action = GetActionIfDoingNothing(hero, rankedMonsters, monstersForWildMana);

                    _actions.Add(hero.Id, action);
                }
            }
        }

        private IAction GetActionIfDoingNothing(Hero hero, IReadOnlyList<Monster> rankedMonsters, IReadOnlyList<Monster> monstersForWildMana)
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

            bestMonster = GetClosestMonster(hero, rankedMonsters);

            if (bestMonster != null)
            {
                if (CanCastControlSpell(hero, bestMonster))
                {
                    return GetSpellAction(SpellType.Control, bestMonster);
                }

                return new MoveAction(bestMonster.Position);
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

        private bool IsMonsterValidForWildMana(Monster monster)
        {
            var distance = GetDistanceFromBase(monster);
            return distance > Constants.BaseRadius && distance < Constants.MaxDistanceFromBaseForHero;
        }

        private float GetDistanceFromBase(Monster monster)
        {
            var distance = (monster.Position - _game.MyPlayer.BasePosition).Length();
            Io.Debug($"Monster Id {monster.Id} :  Distance {distance}");
            return distance;
        }
    }
}

namespace SpringChallenge2022.Agents
{
    internal class SilverBoss
    {
        private Dictionary<int, IAction> _actions;
        private int _availableMana;
        private Game _game;

        public IReadOnlyList<IAction> GetAction(Game game)
        {
            _game = game;
            _actions = new Dictionary<int, IAction>();
            _availableMana = game.MyPlayer.Mana;

            foreach (var monster in _game.Monsters.Values)
            {
                Io.Debug($"MonsterID: {monster.Id}");
            }

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

            foreach (var monster in _game.Monsters.Values)
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

        private bool IsMonsterInRange(
            Hero hero,
            Monster monster,
            int range,
            int minDistance)
        {
            var distance = (hero.Position - monster.Position).Length();
            return distance < range && distance > minDistance;
        }

        private IAction GetActionIfHeroAlreadyTargetingMonster(Hero hero, RankedMonster targetedMonster)
        {
            Io.Debug($"hero {hero.Id} already targeted {targetedMonster.Monster.Id}");

            var spellAction = GetSpellAction(hero, targetedMonster);

            return spellAction ?? new MoveAction(targetedMonster.Monster.Position);
        }

        private IAction? GetSpellAction(Hero hero, RankedMonster targetedMonster)
        {
            if (CanCastWindSpell(hero, targetedMonster))
            {
                return GetSpellAction(SpellType.Wind, null);
            }
            else if (CanCastControlSpell(hero, targetedMonster.Monster))
            {
                var distance = (targetedMonster.Monster.Position - _game.MyPlayer.BasePosition).Length();

                if (distance > Constants.BaseRadius)
                {
                    return GetSpellAction(SpellType.Control, targetedMonster.Monster);
                }
            }

            return null;
        }

        private IAction GetSpellAction(SpellType spellType, Monster? monster)
        {
            _availableMana -= Constants.ManaRequiredForSpell;

            switch (spellType)
            {
                case SpellType.Wind:
                    return new WindSpellAction(_game.OpponentPlayer.BasePosition);
                case SpellType.Control:
                    monster.SetControlled();
                    return new ControlSpellAction(monster.Id, _game.OpponentPlayer.BasePosition);
                case SpellType.Shield:
                default:
                    throw new ArgumentOutOfRangeException(nameof(spellType), spellType, null);
            }
        }

        private bool CanCastWindSpell(Hero hero, RankedMonster rankedMonster)
            => _availableMana >= Constants.ManaRequiredForSpell
               && rankedMonster.Monster.TargetingBase
               && IsMonsterInRange(
                   hero,
                   rankedMonster.Monster,
                   Constants.WindSpellRange,
                   0);

        private bool CanCastControlSpell(Hero hero, Monster rankedMonster)
            => _availableMana >= Constants.ManaRequiredForSpell
               && IsMonsterInRange(
                   hero,
                   rankedMonster,
                   Constants.ControlSpellRange,
                   Constants.HeroDamageDistance)
               && !rankedMonster.ControlledByMe;

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
            var monstersForWildMana = _game.Monsters.Values.Where(monster => !monster.ControlledByMe && IsMonsterValidForWildMana(monster)).ToList();

            foreach (var hero in _game.MyPlayer.Heroes.Values)
            {
                if (!_actions.ContainsKey(hero.Id))
                {
                    var action = GetActionIfDoingNothing(hero, rankedMonsters, monstersForWildMana);

                    _actions.Add(hero.Id, action);
                }
            }
        }

        private IAction GetActionIfDoingNothing(Hero hero, IReadOnlyList<Monster> rankedMonsters, IReadOnlyList<Monster> monstersForWildMana)
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

            bestMonster = GetClosestMonster(hero, rankedMonsters);

            if (bestMonster != null)
            {
                if (CanCastControlSpell(hero, bestMonster))
                {
                    return GetSpellAction(SpellType.Control, bestMonster);
                }

                return new MoveAction(bestMonster.Position);
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

        private bool IsMonsterValidForWildMana(Monster monster)
        {
            var distance = GetDistanceFromBase(monster);
            return distance > Constants.BaseRadius && distance < Constants.MaxDistanceFromBaseForHero;
        }

        private float GetDistanceFromBase(Monster monster)
        {
            var distance = (monster.Position - _game.MyPlayer.BasePosition).Length();
            Io.Debug($"Monster Id {monster.Id} :  Distance {distance}");
            return distance;
        }
    }
}
namespace SpringChallenge2022.Common
{
    public static class Constants
    {
        public const bool IsDebugOn = false;

        public const bool IsForInput = false;

        public const bool IsLocalRun = false;

        public const bool ShowInput = false;
    }
}

namespace SpringChallenge2022.Models
{
    public class Hero
    {
        public int Id { get; }

        public Vector2 Position { get; private set; }

        public Vector2 StartingPosition { get; }

        public Monster? TargetedMonster { get; set; }

        public Hero(int id, Vector2 position, Vector2 startingPosition)
        {
            Id = id;
            Position = position;
            StartingPosition = startingPosition;
        }

        public void Update(Vector2 position)
        {
            Position = position;
        }
    }
}

namespace SpringChallenge2022.Models
{
    public class Monster
    {
        private Vector2 _speed;

        public int Id { get; }

        public Vector2 Position { get; private set; }

        public int Health { get; private set; }

        public bool TargetingBase { get; private set; }

        public int ThreatFor { get; private set; }

        public bool Reinit { get; private set; }

        public bool ControlledByMe { get; private set; }

        public Monster(
            int id,
            Vector2 position,
            int health,
            Vector2 speed,
            bool targetingBase,
            int threatFor)
        {
            Id = id;
            Position = position;
            Health = health;
            _speed = speed;
            TargetingBase = targetingBase;
            ThreatFor = threatFor;
            Reinit = true;
            ControlledByMe = false;
        }

        public void Update(
            Vector2 position,
            int health,
            Vector2 speed,
            bool targetingBase,
            int threatFor)
        {
            Position = position;
            Health = health;
            _speed = speed;
            TargetingBase = targetingBase;
            ThreatFor = threatFor;
            Reinit = true;
        }

        public int GetTurnsToReach(Vector2 position)
            => (int)(((Position - position).Length() - Constants.MonsterBaseDistanceForDamage) / Constants.MonsterSpeed);

        public int GetHitsNeeded()
            => (int)Math.Ceiling((double)Health / Constants.DamagePerHit);

        public void Reset()
        {
            Reinit = false;
        }

        public void SetControlled()
        {
            ControlledByMe = true;
        }
    }
}

namespace SpringChallenge2022.Models
{
    public class Player
    {
        private int _health;

        public int Mana { get; private set; }

        public Dictionary<int, Hero> Heroes { get; } = new Dictionary<int, Hero>();

        public Vector2 BasePosition { get; }

        public Player(Vector2 basePosition)
        {
            BasePosition = basePosition;
        }

        public void Update(int health, int mana, IReadOnlyList<Monster> monsters)
        {
            _health = health;
            Mana = mana;

            RemoveTargetedMonsterForHeroes(monsters);
        }

        private void RemoveTargetedMonsterForHeroes(IReadOnlyList<Monster> monsters)
        {
            foreach (var hero in Heroes.Values)
            {
                if (hero.TargetedMonster != null)
                {
                    if (monsters.All(x => x.Id != hero.TargetedMonster.Id))
                    {
                        Io.Debug($"removing {hero.TargetedMonster.Id} from {hero.Id}");
                        hero.TargetedMonster = null;
                    }
                    else
                    {
                        var monster = monsters.Single(x => x.Id == hero.TargetedMonster.Id);

                        if (monster.ThreatFor != 1)
                        {
                            Io.Debug($"removing {hero.TargetedMonster.Id} from {hero.Id} because its not threat for base anymore");
                            hero.TargetedMonster = null;
                        }
                    }
                }
            }
        }
    }
}
namespace SpringChallenge2022.Models
{
    public class RankedMonster
    {
        public float ThreatLevel { get; }

        public int TurnsToReach { get; }

        public int ShotsNeeded { get; }

        public Monster Monster { get; }

        public RankedMonster(
            Monster monster,
            float threatLevel,
            int turnsToReach,
            int shotsNeeded)
        {
            Monster = monster;
            ThreatLevel = threatLevel;
            TurnsToReach = turnsToReach;
            ShotsNeeded = shotsNeeded;
        }

        public override string ToString()
            => $"Id: {Monster.Id} : ThreatLevel {ThreatLevel} : TurnsToReach {TurnsToReach} : ShotsNeeded {ShotsNeeded}";
    }
}
namespace SpringChallenge2022.Models
{
    public enum SpellType
    {
        Unknown = 0,
        Wind = 1,
        Control = 2,
        Shield = 3
    }
}

namespace SpringChallenge2022.Models
{
    public static class Vector2Extensions
    {
        public static double GetAngle(this Vector2 direction)
        {
            var radians = Math.Atan2(direction.Y, direction.X);
            var degrees = 180 * radians / Math.PI;
            return (360 + Math.Round(degrees)) % 360;
        }

        public static Vector2 GetDirection(float degree)
        {
            var radians = degree * Math.PI / 180;
            return new Vector2((float)Math.Cos(radians), (float)Math.Sin(radians));
        }
    }
}

namespace SpringChallenge2022.Common.Services
{
    public static class Io
    {
        private static StreamReader _file;

        public static void Initialize()
        {
            if (Constants.IsLocalRun)
            {
                _file = new StreamReader(@".\in.txt");
            }
        }

        public static void Debug(string output)
        {
            if (Constants.IsDebugOn || Constants.IsForInput || Constants.ShowInput)
            {
                Console.Error.WriteLine(output);
            }
        }

        public static void WriteLine(string output)
        {
            Console.WriteLine(output);
        }

        public static string ReadLine()
        {
            if (Constants.IsLocalRun)
            {
                return _file.ReadLine();
            }
            else
            {
                var input = Console.ReadLine();

                if (Constants.IsForInput)
                {
                    Debug("IN");
                    Debug(input);
                    Debug("/IN");
                }
                else if(Constants.ShowInput)
                {
                    Debug(input);
                }

                return input;
            }
        }
    }
}