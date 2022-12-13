using System;
using System.Collections.Generic;
using FallChallenge2022.Common.Services;
using System.Linq;
using FallChallenge2022.Agent;
using SpringChallenge2021.Actions;
using FallChallenge2022.Action;
using System.IO;


 // 13/12/2022 11:12


namespace FallChallenge2022
{
    public static class Constants
    {
        public static readonly Random RandomGenerator = new Random(123);
        
    }
}

namespace FallChallenge2022
{
    public class Game
    {
        public int Width { get; }

        public int Height { get; }

        public Player MyPlayer { get; }

        public Game()
        {
            var inputs = Io.ReadLine().Split(' ');
            Width = int.Parse(inputs[0]);
            Height = int.Parse(inputs[1]);
            MyPlayer = new Player();
        }

        public void Parse()
        {
            var inputs = Io.ReadLine().Split(' ');
            var myMatter = int.Parse(inputs[0]);
            var oppMatter = int.Parse(inputs[1]);

            var myUnits = new List<Unit>();

            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    inputs = Io.ReadLine().Split(' ');
                    var scrapAmount = int.Parse(inputs[0]);
                    var owner = int.Parse(inputs[1]); // 1 = me, 0 = foe, -1 = neutral
                    var numberOfUnits = int.Parse(inputs[2]);
                    var recycler = int.Parse(inputs[3]);
                    var canBuild = int.Parse(inputs[4]);
                    var canSpawn = int.Parse(inputs[5]);
                    var inRangeOfRecycler = int.Parse(inputs[6]);

                    for (var k = 0; k < numberOfUnits; k++)
                    {
                        var unit = new Unit(i, j);

                        if (owner == 1)
                        {
                            myUnits.Add(unit);
                        }
                    }
                }
            }

            MyPlayer.ReInit(myMatter, myUnits);
        }
    }
}

namespace FallChallenge2022
{
    public class Player
    {
        private int _matter;

        public IReadOnlyList<Unit> Units { get; private set; }

        public void ReInit(int matter, IReadOnlyList<Unit> units)
        {
            _matter = matter;
            Units = units;
        }
    }
}

namespace FallChallenge2022
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var game = new Game();
            IAgent agent = new SimpleAgentV1();

            // game loop
            while (true)
            {
                game.Parse();

                var actions = agent.GetActions(game);
                var output = string.Join(";", actions.Select(x => x.GetOutputAction()));
                Io.WriteLine(output);
            }
        }
    }
}
namespace FallChallenge2022
{
    public class Unit
    {
        public int PosX { get; }

        public int PosY { get; }

        public Unit(int posY, int posX)
        {
            PosX = posX;
            PosY = posY;
        }
    }
}
namespace SpringChallenge2021.Actions
{
    public interface IAction
    {
        string GetOutputAction();
    }
}

namespace FallChallenge2022.Action
{
    public class MoveAction : IAction
    {
        private readonly int _fromPosX;
        private readonly int _fromPosY;
        private readonly int _toPosX;
        private readonly int _toPosY;

        public MoveAction(
            int fromPosX,
            int fromPosY,
            int toPosX,
            int toPosY)
        {
            _fromPosX = fromPosX;
            _fromPosY = fromPosY;
            _toPosX = toPosX;
            _toPosY = toPosY;
        }

        public string GetOutputAction()
            => $"MOVE 1 {_fromPosX} {_fromPosY} {_toPosX} {_toPosY}";
    }
}
namespace SpringChallenge2021.Actions
{
    public class WaitAction : IAction
    {
        public string GetOutputAction()
        {
            return "WAIT";
        }
    }
}

namespace FallChallenge2022.Agent
{
    public interface IAgent
    {
        IReadOnlyList<IAction> GetActions(Game game);
    }
}

namespace FallChallenge2022.Agent
{
    public class SimpleAgentV1 : IAgent
    {
        public IReadOnlyList<IAction> GetActions(Game game)
        {
            var actions = new List<IAction>();
            Io.Debug(game.Width.ToString());
            Io.Debug(game.Height.ToString());
            
            foreach (var unit in game.MyPlayer.Units)
            {
                var nextX = Constants.RandomGenerator.Next(game.Width - 1);
                var nextY = Constants.RandomGenerator.Next(game.Height - 1);
                actions.Add(new MoveAction(unit.PosX, unit.PosY, nextX, nextY));
            }

            if (actions.Count == 0)
            {
                actions.Add(new WaitAction());
            }
            
            return actions;
        }
    }
}
namespace FallChallenge2022.Common
{
    public static class Constants
    {
        public const bool IsDebugOn = true;

        public const bool IsForInput = false;

        public const bool IsLocalRun = false;

        public const bool ShowInput = false;
    }
}

namespace FallChallenge2022.Common.Services
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