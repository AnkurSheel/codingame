using System;
using FallChallenge2022.Common.Services;
using System.IO;


 // 13/12/2022 07:14


namespace FallChallenge2022
{
    public static class Constants
    {
        public static readonly Random RandomGenerator = new Random(123);
        
    }
}

namespace FallChallenge2022
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string[] inputs;
            inputs = Console.ReadLine().Split(' ');
            int width = int.Parse(inputs[0]);
            int height = int.Parse(inputs[1]);

            // game loop
            while (true)
            {
                inputs = Io.ReadLine().Split(' ');
                int myMatter = int.Parse(inputs[0]);
                int oppMatter = int.Parse(inputs[1]);
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        inputs = Io.ReadLine().Split(' ');
                        int scrapAmount = int.Parse(inputs[0]);
                        int owner = int.Parse(inputs[1]); // 1 = me, 0 = foe, -1 = neutral
                        int units = int.Parse(inputs[2]);
                        int recycler = int.Parse(inputs[3]);
                        int canBuild = int.Parse(inputs[4]);
                        int canSpawn = int.Parse(inputs[5]);
                        int inRangeOfRecycler = int.Parse(inputs[6]);
                    }
                }

                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");

                Io.WriteLine("WAIT");
            }
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