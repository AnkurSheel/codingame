namespace JoinThePac.Models
{
    public class Pac
    {
        public int Id { get; }

        public Coordinate Position { get; }

        private Coordinate _previousPosition;

        public bool IsAlive { get; private set; }

        public Pac(int id, int x, int y)
        {
            Id = id;
            Position = new Coordinate(x, y);
            IsAlive = true;
            _previousPosition = new Coordinate(-1, -1);
        }

        public bool IsInSamePosition()
        {
            return _previousPosition.IsSame(Position);
        }

        public void Update(int x, int y)
        {
            _previousPosition = new Coordinate(Position);
            Position.Update(x, y);
            IsAlive = true;
        }

        public void Reset()
        {
            IsAlive = false;
        }
    }
}
