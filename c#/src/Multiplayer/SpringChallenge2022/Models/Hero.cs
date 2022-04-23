using SpringChallenge2022.Agents;

namespace SpringChallenge2022.Models
{
    public class Hero : Entity
    {
        public Vector StartingPosition { get; }

        public Monster TargetedMonster { get; set; }

        public Hero(int id, Vector position) : base(id, position)
        {
            StartingPosition = position;
        }

    }
}
