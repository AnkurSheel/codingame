using System;
using System.IO;


 // 23/04/2022 05:45


/**
 * Auto-generated code below aims at helping you parse
 * the standard input according to the problem statement.
 **/
internal class Player
{
    private static void Main(string[] args)
    {
        string[] inputs;
        inputs = Console.ReadLine().Split(' ');
        var baseX = int.Parse(inputs[0]); // The corner of the map representing your base
        var baseY = int.Parse(inputs[1]);
        var heroesPerPlayer = int.Parse(Console.ReadLine()); // Always 3

        // game loop
        while (true)
        {
            for (var i = 0; i < 2; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var health = int.Parse(inputs[0]); // Each player's base health
                var mana = int.Parse(inputs[1]); // Ignore in the first league; Spend ten mana to cast a spell
            }

            var entityCount = int.Parse(Console.ReadLine()); // Amount of heros and monsters you can see

            for (var i = 0; i < entityCount; i++)
            {
                inputs = Console.ReadLine().Split(' ');
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
            }

            for (var i = 0; i < heroesPerPlayer; i++)
            {
                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");

                // In the first league: MOVE <x> <y> | WAIT; In later leagues: | SPELL <spellParams>;
                Console.WriteLine("WAIT");
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