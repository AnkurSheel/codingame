using System.Numerics;

namespace SpringChallenge2022.Models
{
    public class Hero
    {
        public int Id { get; }

        public Vector2 Position { get; private set; }

        public Vector2 StartingPosition { get; }

        public Monster? TargetedMonster { get; set; }

        public Hero(int id, Vector2 position, Vector2 startingPosition)
        {
            Id = id;
            Position = position;
            StartingPosition = startingPosition;
        }

        public void Update(Vector2 position)
        {
            Position = position;
        }
    }
}
