using System.Collections.Generic;
using System.Numerics;

namespace SpringChallenge2022.Models
{
    public class Hero
    {
        public int Id { get; }

        public Vector2 Position { get; private set; }

        public Vector2 StartingPosition { get; }

        public Monster? TargetedMonster { get; set; }

        public Hero(int id, Vector2 position, Vector2 startingPosition)
        {
            Id = id;
            Position = position;
            StartingPosition = startingPosition;
        }

        public void Update(Vector2 position)
        {
            Position = position;
        }

        public bool CanCastWindSpell(int availableMana, RankedMonster rankedMonster)
        {
            var distance = Position.GetDistance(rankedMonster.Monster.Position);

            return availableMana >= Constants.ManaRequiredForSpell && rankedMonster.Monster.TargetingBase && distance < Constants.WindSpellRange;
        }

        public bool CanCastControlSpell(int availableMana, Monster monster)
        {
            var distance = Position.GetDistance(monster.Position);

            return availableMana >= Constants.ManaRequiredForSpell && distance < Constants.WindSpellRange && distance > Constants.HeroDamageDistance && !monster.ControlledByMe;
        }

        public Monster? GetClosestMonster(IReadOnlyList<Monster> monsters)
        {
            Monster bestMonster = null;
            var bestMonsterDistance = float.MaxValue;

            foreach (var monster in monsters)
            {
                var distance = Position.GetDistance(monster.Position);

                if (distance < bestMonsterDistance)
                {
                    bestMonsterDistance = distance;
                    bestMonster = monster;
                }
            }

            return bestMonster;
        }
    }
}
