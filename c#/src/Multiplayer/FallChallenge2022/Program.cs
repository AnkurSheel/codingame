using System;
using FallChallenge2022.Common.Services;

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
