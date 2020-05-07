namespace JoinThePac.Models
{
    public class Pac
    {
        public int Id { get; }

        public int X { get; }

        public int Y { get; }

        public Pac(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }
}
