namespace JoinThePac.Models
{
    public class Pac
    {
        public int Id { get; }

        public int X { get; private set; }

        public int Y { get; private set; }

        private int _previousX = -1;

        private int _previousY = -1;

        public Pac(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }

        public bool IsInSamePosition()
        {
            return _previousX == X && _previousY == Y;
        }

        public void Update(int x, int y)
        {
            _previousX = X;
            _previousY = Y;
            X = x;
            Y = y;
        }
    }
}
