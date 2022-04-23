using SpringChallenge2022;
using System;
using System.Collections.Generic;
using System.Linq;
using SpringChallenge2022.Common.Services;
using System.IO;


 // 23/04/2022 06:30


/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
public class Entity
{
    public int Id;
    public EntityType Type;
    public int X, Y;
    public int ShieldLife;
    public int IsControlled;
    public int Health;
    public int Vx, Vy;
    public bool TargetingBase;
    public int ThreatFor;

    public Entity(
        int id,
        EntityType type,
        int x,
        int y,
        int shieldLife,
        int isControlled,
        int health,
        int vx,
        int vy,
        bool targetingBase,
        int threatFor)
    {
        Id = id;
        Type = type;
        X = x;
        Y = y;
        ShieldLife = shieldLife;
        IsControlled = isControlled;
        Health = health;
        Vx = vx;
        Vy = vy;
        TargetingBase = targetingBase;
        ThreatFor = threatFor;
    }
}
namespace SpringChallenge2022
{
    public enum EntityType
    {
        MONSTER = 0,
        HERO = 1,
        OPPONENT_HERO = 2
    }
}

internal class Player
{
    private static void Main(string[] args)
    {
        string[] inputs;
        inputs = Io.ReadLine().Split(' ');

        // base_x,base_y: The corner of the map representing your base
        var baseX = int.Parse(inputs[0]);
        var baseY = int.Parse(inputs[1]);

        // heroesPerPlayer: Always 3
        var heroesPerPlayer = int.Parse(Io.ReadLine());

        // game loop
        while (true)
        {
            inputs = Io.ReadLine().Split(' ');
            var myHealth = int.Parse(inputs[0]); // Your base health
            var myMana = int.Parse(inputs[1]); // Ignore in the first league; Spend ten mana to cast a spell

            inputs = Io.ReadLine().Split(' ');
            var oppHealth = int.Parse(inputs[0]);
            var oppMana = int.Parse(inputs[1]);

            var entityCount = int.Parse(Io.ReadLine()); // Amount of heros and monsters you can see

            var myHeroes = new List<Entity>(entityCount);
            var oppHeroes = new List<Entity>(entityCount);
            var monsters = new List<Entity>(entityCount);

            for (var i = 0; i < entityCount; i++)
            {
                inputs = Io.ReadLine().Split(' ');
                var id = int.Parse(inputs[0]); // Unique identifier
                var type = Enum.Parse<EntityType>(inputs[1]); // 0=monster, 1=your hero, 2=opponent hero
                var x = int.Parse(inputs[2]); // Position of this entity
                var y = int.Parse(inputs[3]);
                var shieldLife = int.Parse(inputs[4]); // Ignore for this league; Count down until shield spell fades
                var isControlled = int.Parse(inputs[5]); // Ignore for this league; Equals 1 when this entity is under a control spell
                var health = int.Parse(inputs[6]); // Remaining health of this monster
                var vx = int.Parse(inputs[7]); // Trajectory of this monster
                var vy = int.Parse(inputs[8]);
                var nearBase = int.Parse(inputs[9]); // 0=monster with no target yet, 1=monster targeting a base
                var threatFor = int.Parse(inputs[10]); // Given this monster's trajectory, is it a threat to 1=your base, 2=your opponent's base, 0=neither

                var entity = new Entity(
                    id,
                    type,
                    x,
                    y,
                    shieldLife,
                    isControlled,
                    health,
                    vx,
                    vy,
                    nearBase == 1,
                    threatFor);

                switch (type)
                {
                    case EntityType.MONSTER:
                        monsters.Add(entity);
                        break;
                    case EntityType.HERO:
                        myHeroes.Add(entity);
                        break;
                    case EntityType.OPPONENT_HERO:
                        oppHeroes.Add(entity);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            var rankedMonsters = new List<Tuple<int, Entity>>();

            foreach (var monster in monsters)
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

                var dX = baseX - monster.X;
                var dY = baseY - monster.Y;
                var distance = dX * dX + dY * dY;
                var distanceScore = 500 * (1 / distance + 1);

                rankedMonsters.Add(new Tuple<int, Entity>(threatLevel + distanceScore, monster));
            }

            rankedMonsters = rankedMonsters.OrderByDescending(x => x.Item1).ToList();

            for (var index = 0; index < myHeroes.Count; index++)
            {
                if (rankedMonsters.Count > index)
                {
                    var monster = rankedMonsters[index].Item2;
                    Io.WriteLine($"MOVE {monster.X} {monster.Y}");
                }
                else
                {
                    Io.WriteLine($"WAIT");
                }
            }
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

        public const bool ShowInput = false;
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