﻿using SpringChallenge2022.Agents;

namespace SpringChallenge2022.Models
{
    public class Monster : Entity
    {
        private int _health;
        private Vector _speed;

        public bool TargetingBase { get; }

        public int ThreatFor { get; }

        public Monster(
            int id,
            Vector position,
            int health,
            Vector speed,
            bool targetingBase,
            int threatFor) : base(id, position)
        {
            _health = health;
            _speed = speed;
            TargetingBase = targetingBase;
            ThreatFor = threatFor;
        }
    }
}