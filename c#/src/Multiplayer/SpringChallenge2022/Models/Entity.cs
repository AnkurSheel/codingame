using SpringChallenge2022.Agents;

namespace SpringChallenge2022.Models
{
    public abstract class Entity
    {
        public int Id { get; }

        public Vector Position { get; private set; }

        protected Entity(int id, Vector position)
        {
            Id = id;
            Position = position;
        }

        public void UpdatePosition(Vector position)
        {
            Position = position;
        }
    }
}
