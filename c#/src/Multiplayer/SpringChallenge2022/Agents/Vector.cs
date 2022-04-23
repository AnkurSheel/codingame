namespace SpringChallenge2022.Agents
{
    public class Vector
    {
        public int X { get; }

        public int Y { get; }

        public Vector(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int GetDistanceSquared(Vector other)
        {
            var dX = X - other.X;
            var dY = Y - other.Y;
            var distance = dX * dX + dY * dY;
            return distance;
        }
    }
}
