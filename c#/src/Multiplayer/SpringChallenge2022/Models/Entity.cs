using SpringChallenge2022.Agents;

namespace SpringChallenge2022.Models
{
    public abstract class Entity
    {
        private int _id;

        public Vector Position { get; }

        protected Entity(int id, Vector position)
        {
            _id = id;
            Position = position;
        }
    }
}
