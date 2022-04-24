using System.Collections.Generic;

namespace SpringChallenge2022.Models
{
    public class RankedMonster
    {
        public float ThreatLevel { get; }

        public int TurnsToReach { get; }

        public int ShotsNeeded { get; }

        public Monster Monster { get; }

        public RankedMonster(
            Monster monster,
            float threatLevel,
            int turnsToReach,
            int shotsNeeded)
        {
            Monster = monster;
            ThreatLevel = threatLevel;
            TurnsToReach = turnsToReach;
            ShotsNeeded = shotsNeeded;
        }

        public Hero GetClosestHero(IReadOnlyList<Hero> heroes)
        {
            Hero bestHero = null;
            var bestHeroDistance = float.MaxValue;

            foreach (var hero in heroes)
            {
                var distance = hero.Position.GetDistance(Monster.Position);

                if (distance < bestHeroDistance)
                {
                    bestHeroDistance = distance;
                    bestHero = hero;
                }
            }

            return bestHero;
        }

        public override string ToString()
            => $"Id: {Monster.Id} : ThreatLevel {ThreatLevel} : TurnsToReach {TurnsToReach} : ShotsNeeded {ShotsNeeded}";
    }
}
