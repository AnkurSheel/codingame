using System;
using System.Diagnostics;

namespace JoinThePac.Models
{
    [DebuggerDisplay("x = {X} y = {Y}")]
    public class Coordinate
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Coordinate(Coordinate position)
        {
            X = position.X;
            Y = position.Y;
        }

        public int Manhattan(Coordinate pos)
        {
            return Math.Abs(X - pos.X) + Math.Abs(Y - pos.Y);
        }

        public override string ToString()
        {
            return $"{X} {Y}";
        }

        public bool IsSame(Coordinate pos)
        {
            return X == pos.X && Y == pos.Y;
        }

        public void Update(Coordinate pos)
        {
            Update(pos.X, pos.Y);
        }

        public void Update(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
