using System.Collections.Generic;
using System.Diagnostics;

namespace JoinThePac.Models
{
    [DebuggerDisplay("Position = {Position} type={Type} value ={PelletValue}")]
    public class Cell
    {
        public Cell(int x, int y, CellType cellType)
        {
            Position = new Coordinate(x, y);
            Type = cellType;
            Neighbours = new Dictionary<Direction, Cell>();
            VisibleCells = new HashSet<Cell>();
            PelletValue = 1;
        }

        public int PelletValue { get; set; }

        public Coordinate Position { get; }

        public CellType Type { get; }

        public Dictionary<Direction, Cell> Neighbours { get; set; }

        public HashSet<Cell> VisibleCells { get; set; }

        public bool HasPellet => PelletValue > 0;

        protected bool Equals(Cell other)
        {
            return Equals(Position, other.Position);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Cell)obj);
        }

        public override int GetHashCode()
        {
            return Position != null
                       ? Position.GetHashCode()
                       : 0;
        }
    }
}
