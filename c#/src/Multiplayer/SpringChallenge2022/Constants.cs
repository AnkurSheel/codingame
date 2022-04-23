using System;
using System.Numerics;

namespace SpringChallenge2022
{
    public static class Constants
    {
        public static readonly Random RandomGenerator = new Random(123);
        public static readonly Vector2 BottomRightMap = new Vector2(17630, 9000);
        public const int MonsterSpeed = 400;
        public const int DamagePerHit = 2;
        public const int ManaRequiredForSpell = 10;
        public const int ControlSpellRange = 2200;

    }
}
