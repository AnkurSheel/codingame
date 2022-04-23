using System.Numerics;

namespace SpringChallenge2022.Models
{
    public class Monster
    {
        private int _health;
        private Vector2 _speed;

        public int Id { get; }

        public Vector2 Position { get; }

        public bool TargetingBase { get; }

        public int ThreatFor { get; }

        public Monster(
            int id,
            Vector2 position,
            int health,
            Vector2 speed,
            bool targetingBase,
            int threatFor)
        {
            Id = id;
            Position = position;
            _health = health;
            _speed = speed;
            TargetingBase = targetingBase;
            ThreatFor = threatFor;
        }
    }
}
