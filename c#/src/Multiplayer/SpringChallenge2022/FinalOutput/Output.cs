using System;
using SpringChallenge2022.Models;
using System.Collections.Generic;
using SpringChallenge2022;
using SpringChallenge2022.Common.Services;
using SpringChallenge2022.Agents;
using System.Linq;
using SpringChallenge2022.Actions;
using System.IO;


 // 24/04/2022 09:40


namespace SpringChallenge2022
{
    public static class Constants
    {
        public static readonly Random RandomGenerator = new Random(123);
        public static readonly Vector BottomRightMap = new Vector(17630, 9000);
    }
}

internal class Game
{
    private Base _opponentBase;
    private int _heroesPerPlayer;

    private List<Hero> _opponentHeroes;

    public Base MyBase { get; private set; }

    public Dictionary<int, Hero> MyHeroes { get; } = new Dictionary<int, Hero>();

    public Dictionary<int, Monster> Monsters { get; private set; }

    public void Initialize()
    {
        var inputs = Io.ReadLine().Split(' ');

        // base_x,base_y: The corner of the map representing your base
        var baseX = int.Parse(inputs[0]);
        var baseY = int.Parse(inputs[1]);

        MyBase = new Base(new Vector(baseX, baseY));
        _opponentBase = new Base(new Vector(Constants.BottomRightMap.X - baseX, Constants.BottomRightMap.Y - baseY));

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

        foreach (var hero in MyHeroes.Values)
        {
            if (hero.TargetedMonster != null && !Monsters.ContainsKey(hero.TargetedMonster.Id))
            {
                Io.Debug($"removing {hero.TargetedMonster.Id} from {hero.Id}");
                hero.TargetedMonster = null;
            }
        }
    }

    private void ReInitEntities()
    {
        var entityCount = int.Parse(Io.ReadLine()); // Amount of heros and monsters you can see

        _opponentHeroes = new List<Hero>(entityCount);
        Monsters = new Dictionary<int, Monster>();

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

            switch (type)
            {
                case 0:
                    var monster = new Monster(
                        id,
                        new Vector(x, y),
                        health,
                        new Vector(vx, vy),
                        nearBase == 1,
                        threatFor);
                    Monsters.Add(id, monster);
                    break;
                case 1:
                    if (MyHeroes.ContainsKey(id))
                    {
                        MyHeroes[id].Update(new Vector(x, y));
                    }
                    else
                    {
                        MyHeroes.Add(id, new Hero(id, new Vector(x, y)));
                    }

                    break;
                case 2:
                    _opponentHeroes.Add(new Hero(id, new Vector(x, y)));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}

internal class Player
{
    private static void Main(string[] args)
    {
        Io.Initialize();

        var game = new Game();
        var agent = new BronzeBoss();

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
        private const int Range = 2200;
        private readonly int _id;
        private readonly Vector _targetPosition;

        public ControlSpellAction(int id, Vector targetPosition)
        {
            _id = id;
            _targetPosition = targetPosition;
        }

        public string GetOutputAction()
            => $"SPELL CONTROL {_id} {_targetPosition.X} {_targetPosition.Y}";
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
        private readonly Vector _position;

        public MoveAction(Vector position)
        {
            _position = position;
        }

        public string GetOutputAction()
            => $"MOVE {_position.X} {_position.Y}";
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
        private readonly Vector _targetPosition;

        public WindSpellAction(Vector targetPosition)
        {
            _targetPosition = targetPosition;
        }

        public string GetOutputAction()
            => $"SPELL WIND {_targetPosition.X} {_targetPosition.Y}";
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

                var distance = game.MyBase.Position.GetDistanceSquared(monster.Position);
                var distanceScore = 500 * (1 / distance + 1);

                rankedMonsters.Add(new Tuple<int, Monster>(threatLevel + distanceScore, monster));
            }

            rankedMonsters = rankedMonsters.OrderByDescending(x => x.Item1).ToList();

            for (var index = 0; index < game.MyHeroes.Count; index++)
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
        public IReadOnlyList<IAction> GetAction(Game game)
        {
            var actions = new Dictionary<int, IAction>();

            var rankedMonsters = GetRankedMonsters(game);

            Io.Debug($"RankedMonsters {rankedMonsters.Count}");

            foreach (var monster in rankedMonsters)
            {
                Io.Debug($"Evaluating {monster.Id}");

                var heroTargetingMonster = game.MyHeroes.Values.SingleOrDefault(x => x.TargetedMonster?.Id == monster.Id);

                if (heroTargetingMonster != null)
                {
                    Io.Debug($"hero {heroTargetingMonster.Id} already targeted {monster.Id}");
                    actions.Add(heroTargetingMonster.Id, new MoveAction(monster.Position));
                    continue;
                }

                Hero bestHero = null;
                var bestHeroDistance = int.MaxValue;

                foreach (var (_, hero) in game.MyHeroes)
                {
                    if (hero.TargetedMonster != null)
                    {
                        continue;
                    }

                    var distance = hero.Position.GetDistanceSquared(monster.Position);

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

            foreach (var (_, hero) in game.MyHeroes)
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
            }

            return rankedMonsters.OrderByDescending(x => x.Item1).Select(x => x.Item2).ToList();
        }
    }
}
namespace SpringChallenge2022.Common
{
    public static class Constants
    {
        public const bool IsDebugOn = true;

        public const bool IsForInput = false;

        public const bool IsLocalRun = false;

        public const bool ShowInput = true;
    }
}

namespace SpringChallenge2022.Models
{
    public class Base
    {
        public Vector Position { get; }

        public Base(Vector position)
        {
            Position = position;
        }
    }
}
namespace SpringChallenge2022.Models
{
    public class Hero
    {
        public int Id { get; }

        public Vector Position { get; private set; }

        public Vector StartingPosition { get; }

        public Monster? TargetedMonster { get; set; }

        public Hero(int id, Vector position)
        {
            Id = id;
            Position = position;

            StartingPosition = position.X > 8000
                ? new Vector(Constants.BottomRightMap.X - 3500, Constants.BottomRightMap.Y - 3500)
                : new Vector(3500, 3500);
        }

        public void Update(Vector position)
        {
            Position = position;
        }
    }
}

namespace SpringChallenge2022.Models
{
    public class Monster
    {
        private int _health;
        private Vector _speed;

        public int Id { get; }

        public Vector Position { get; private set; }

        public bool TargetingBase { get; }

        public int ThreatFor { get; }

        public Monster(
            int id,
            Vector position,
            int health,
            Vector speed,
            bool targetingBase,
            int threatFor)
        {
            Id = id;
            Position = position;
            _health = health;
            _speed = speed;
            TargetingBase = targetingBase;
            ThreatFor = threatFor;
        }
    }
}
namespace SpringChallenge2022.Models
{
    public class Vector
    {
        public int X { get; }

        public int Y { get; }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int GetDistanceSquared(Vector other)
        {
            var dX = X - other.X;
            var dY = Y - other.Y;
            var distance = dX * dX + dY * dY;
            return distance;
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