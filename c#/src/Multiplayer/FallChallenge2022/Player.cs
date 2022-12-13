using System.Collections.Generic;
using FallChallenge2022.Common.Services;
using FallChallenge2022.Models;

namespace FallChallenge2022
{
    public class Player
    {
        public int Matter { get; private set; }

        public IReadOnlyList<Unit> Units { get; private set; }

        public List<Position> Tiles { get; private set; }

        public void ReInit(int matter, IReadOnlyList<Unit> units, List<Position> tiles)
        {
            Matter = matter;
            Units = units;
            Tiles = tiles;
            Io.Debug($"Tiles {tiles.Count}");
        }

        public bool CanSpawn()
        {
            return Matter > 10;
        }

        public Position GetRandomTilePosition()
        {
            return Tiles[Constants.RandomGenerator.Next(Tiles.Count)];
        }
    }
}
