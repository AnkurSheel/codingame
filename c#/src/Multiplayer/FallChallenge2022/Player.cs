using System.Collections.Generic;
using FallChallenge2022.Common.Services;
using FallChallenge2022.Models;

namespace FallChallenge2022
{
    public class Player
    {
        private List<Position> _tiles;

        public int Matter { get; private set; }

        public IReadOnlyList<Unit> Units { get; private set; }

        public void ReInit(int matter, IReadOnlyList<Unit> units, List<Position> tiles)
        {
            Matter = matter;
            Units = units;
            _tiles = tiles;
        }

        public Position GetRandomTilePosition()
        {
            Io.Debug($"{Units.Count} {_tiles.Count}");
           return _tiles[Constants.RandomGenerator.Next(_tiles.Count)];
        }
    }
}
