using System.Collections.Generic;
using FallChallenge2022.Common.Services;

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
