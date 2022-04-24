using System;
using System.Numerics;

namespace SpringChallenge2022.Models
{
    public class Monster
    {

        private Vector2 _speed;

        public int Id { get; }

        public Vector2 Position { get; }

        public int Health { get; }
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
            Health = health;
            _speed = speed;
            TargetingBase = targetingBase;
            ThreatFor = threatFor;
        }

        public int GetTurnsToReach(Vector2 position)
            => (int)(((Position - position).Length() - Constants.MonsterBaseDistanceForDamage) / Constants.MonsterSpeed);

        public int GetHitsNeeded()
        {
            return (int)Math.Ceiling((double)(Health) / Constants.DamagePerHit);
        }
    }
}
