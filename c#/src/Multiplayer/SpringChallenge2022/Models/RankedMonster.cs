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

        public override string ToString()
            => $"Id: {Monster.Id} : ThreatLevel {ThreatLevel} : TurnsToReach {TurnsToReach} : ShotsNeeded {ShotsNeeded}";
    }
}
