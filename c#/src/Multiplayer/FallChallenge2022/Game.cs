using System.Collections.Generic;
using FallChallenge2022.Common.Services;
using FallChallenge2022.Models;

namespace FallChallenge2022
{
    public class Game
    {
        private readonly Tile[,] _board;

        public int Width { get; }

        public int Height { get; }

        public Player MyPlayer { get; }

        public Game()
        {
            var inputs = Io.ReadLine().Split(' ');
            Width = int.Parse(inputs[0]);
            Height = int.Parse(inputs[1]);
            MyPlayer = new Player();

            _board = new Tile[Width, Height];
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
                    var tile = new Tile(j, i);

                    _board[j, i] = tile;

                    for (var k = 0; k < tile.NumberOfUnits; k++)
                    {
                        if (tile.Owner == 1)
                        {
                            var unit = new Unit(tile);

                            myUnits.Add(unit);
                        }
                    }
                }
            }

            MyPlayer.ReInit(myMatter, myUnits);
        }

        public Tile? GetTileAt(Position position)
        {
            if (position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height)
            {
                return _board[position.X, position.Y];
            }

            return null;
        }
    }
}
