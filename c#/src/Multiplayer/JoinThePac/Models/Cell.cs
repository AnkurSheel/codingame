using System.Collections.Generic;
using System.Diagnostics;

namespace JoinThePac.Models
{
    [DebuggerDisplay("x = {X} y = {Y} type={Type}")]
    public class Cell
    {
        public Cell(int x, int y, CellType cellType)
        {
            Position = new Coordinate(x, y);
            Type = cellType;
            Neighbours = new Dictionary<Direction, Cell>();
            Pellet = 0;
        }

        public Coordinate Position { get; }

        public CellType Type { get; }

        public Dictionary<Direction, Cell> Neighbours { get; set; }

        public int Pellet { get; set; }

        public bool HasPellet => Pellet > 0;
    }
}
