namespace JoinThePac.Models
{
    public class Pac
    {
        public int Id { get; }

        public Coordinate Position { get; }

        private Coordinate _previousPosition;

        public bool IsAlive { get; private set; }

        public PacType Type { get; private set; }

        public int AbilityCooldown { get; private set; }

        public int SpeedTurnsLeft { get; private set; }

        public Pac(int id, int x, int y, PacType type)
        {
            Id = id;
            Type = type;
            Position = new Coordinate(x, y);
            IsAlive = true;
            _previousPosition = new Coordinate(-1, -1);
        }

        public bool IsInSamePosition()
        {
            return _previousPosition.IsSame(Position);
        }

        public void Update(int x, int y, PacType type, int speedTurnsLeft, int abilityCooldown)
        {
            _previousPosition = new Coordinate(Position);
            Position.Update(x, y);
            IsAlive = true;
            Type = type;
            SpeedTurnsLeft = speedTurnsLeft;
            AbilityCooldown = abilityCooldown;
        }

        public void Reset()
        {
            IsAlive = false;
        }
    }
}
