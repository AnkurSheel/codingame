using SpringChallenge2022.Agents;

namespace SpringChallenge2022.Models
{
    public class Hero
    {
        public int Id { get; }

        public Vector Position { get; private set; }

        public Vector StartingPosition { get; }

        public Monster TargetedMonster { get; set; }

        public Hero(int id, Vector position)
        {
            Id = id;
            Position = position;

            StartingPosition = position.X > 8000
                ? new Vector(Constants.BottomRightMap.X - 3500, Constants.BottomRightMap.Y - 3500)
                : new Vector(3500, 3500);
        }

        public void Update(Vector position)
        {
            Position = position;
        }
    }
}
