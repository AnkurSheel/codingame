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
            StartingPosition = position;
        }

        public void Update(Vector position)
        {
            Position = position;
        }
    }
}
