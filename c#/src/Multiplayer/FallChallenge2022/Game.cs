using System.Collections.Generic;
using FallChallenge2022.Common.Services;

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
