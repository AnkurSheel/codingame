using System;
using System.Numerics;

namespace SpringChallenge2022
{
    public static class Constants
    {
        public static readonly Random RandomGenerator = new Random(123);
        public static readonly Vector2 BottomRightMap = new Vector2(17630, 9000);
        public const int MonsterSpeed = 400;
        public const int MonsterBaseDistanceForDamage = 300;
        public const int DamagePerHit = 2;
        public const int ManaRequiredForSpell = 10;
        public const int ControlSpellRange = 2200;
        public const int DistanceFromBaseForStartingPosition = 6000;
        public const float DistanceBaseScore = 10000.0f;
        public const float ShotsNeededBaseScore = 20.0f;
        public const float TargetingBaseBaseScore = 1000.0f;
        public const float NonTargetingBaseBaseScore = 500.0f;
    }
}
