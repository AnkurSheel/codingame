using System;
using System.Numerics;
using SpringChallenge2022.Common.Services;

namespace SpringChallenge2022.Models
{
    public class Monster
    {
        private Vector2 _speed;

        public int Id { get; }

        public Vector2 Position { get; private set; }

        public int Health { get; private set; }

        public bool TargetingBase { get; private set; }

        public int ThreatFor { get; private set; }

        public bool Reinit { get; private set; }

        public bool ControlledByMe { get; private set; }

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
            Reinit = true;
            ControlledByMe = false;
        }

        public void Update(
            Vector2 position,
            int health,
            Vector2 speed,
            bool targetingBase,
            int threatFor)
        {
            Position = position;
            Health = health;
            _speed = speed;
            TargetingBase = targetingBase;
            ThreatFor = threatFor;
            Reinit = true;
        }

        public int GetTurnsToReach(Vector2 position)
        {
            var distance = Position.GetDistance(position);
            return (int)((distance - Constants.MonsterBaseDistanceForDamage) / Constants.MonsterSpeed);
        }

        public int GetHitsNeeded()
            => (int)Math.Ceiling((double)Health / Constants.DamagePerHit);

        public void Reset()
        {
            Reinit = false;
        }

        public void SetControlled()
        {
            ControlledByMe = true;
        }

        public bool IsValidForWildMana(Vector2 basePosition)
        {
            var distance = Position.GetDistance(basePosition);
            Io.Debug($"Monster Id {Id} :  Distance {distance}");
            return distance > Constants.BaseRadius && distance < Constants.MaxDistanceFromBaseForHero;
        }


    }
}
