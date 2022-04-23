using System.Numerics;
using SpringChallenge2022.Common.Services;

namespace SpringChallenge2022.Models
{
    public class Hero
    {
        public int Id { get; }

        public Vector2 Position { get; private set; }

        public Vector2 StartingPosition { get; }

        public Monster? TargetedMonster { get; set; }

        public Hero(int id, Vector2 position)
        {
            Id = id;
            Position = position;

            var direction = position.X > 8000
                ? new Vector2(Constants.BottomRightMap.X - position.X, Constants.BottomRightMap.Y - position.Y)
                : new Vector2(position.X, position.Y);

            direction = direction / direction.Length();

            Io.Debug($"Direction {id} - {direction.X}, {direction.Y}");

            StartingPosition = position.X > 8000
                ? new Vector2(Constants.BottomRightMap.X - 3500, Constants.BottomRightMap.Y - 3500)
                : new Vector2(3500, 3500);
        }

        //17000 -position.x

        public void Update(Vector2 position)
        {
            Position = position;
        }
    }
}
