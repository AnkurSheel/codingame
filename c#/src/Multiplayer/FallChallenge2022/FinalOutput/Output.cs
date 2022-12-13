using System;
using System.Collections.Generic;
using FallChallenge2022.Common.Services;
using FallChallenge2022.Agent;
using SpringChallenge2021.Actions;
using System.IO;


 // 13/12/2022 10:49


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
        private readonly int _width;
        private readonly int _height;
        private readonly Player _myPlayer;

        public Game()
        {
            var inputs = Io.ReadLine().Split(' ');
            _width = int.Parse(inputs[0]);
            _height = int.Parse(inputs[1]);
            _myPlayer = new Player();
        }

        public void Parse()
        {
            var inputs = Io.ReadLine().Split(' ');
            var myMatter = int.Parse(inputs[0]);
            var oppMatter = int.Parse(inputs[1]);

            var myUnits = new List<Unit>();

            for (var i = 0; i < _height; i++)
            {
                for (var j = 0; j < _width; j++)
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

            _myPlayer.ReInit(myMatter, myUnits);
        }
    }
}

namespace FallChallenge2022
{
    internal class Player
    {
        private int _matter;
        private IReadOnlyList<Unit> _units;

        public void ReInit(int matter, IReadOnlyList<Unit> units)
        {
            _matter = matter;
            _units = units;
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

                var action = agent.GetAction(game);
                Io.WriteLine(action.GetOutputAction());
            }
        }
    }
}
namespace FallChallenge2022
{
    internal class Unit
    {
        private readonly int _posX;
        private readonly int _posY;

        public Unit(int posX, int posY)
        {
            _posX = posX;
            _posY = posY;
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
        IAction GetAction(Game game);
    }
}

namespace FallChallenge2022.Agent
{
    public class SimpleAgentV1 : IAgent
    {
        public IAction GetAction(Game game)
        {
            return new WaitAction();
        }
    }
}
namespace FallChallenge2022.Common
{
    public static class Constants
    {
        public const bool IsDebugOn = false;

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