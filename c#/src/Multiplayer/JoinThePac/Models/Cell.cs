using System.Collections.Generic;

namespace JoinThePac.Models
{
    public class Cell
    {
        public Cell(int x, int y, CellType cellType)
        {
            X = x;
            Y = y;
            Type = cellType;
            Neighbours = new Dictionary<Direction, Cell>();
            Pellet = 0;
        }

        public int X { get; }

        public int Y { get; }

        public CellType Type { get; }

        public Dictionary<Direction, Cell> Neighbours { get; set; }

        public int Pellet { get; set; }

        public bool HasPellet => Pellet > 0;
    }
}
