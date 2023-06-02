using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SpringChallenge2022.Common.Services;

namespace SpringChallenge2022.Models
{
    public class Player
    {
        private int _health;

        public int Mana { get; private set; }

        public Dictionary<int, Hero> Heroes { get; } = new Dictionary<int, Hero>();

        public Vector2 BasePosition { get; }

        public Player(Vector2 basePosition)
        {
            BasePosition = basePosition;
        }

        public void Update(int health, int mana, IReadOnlyList<Monster> monsters)
        {
            _health = health;
            Mana = mana;

            RemoveTargetedMonsterForHeroes(monsters);
        }

        private void RemoveTargetedMonsterForHeroes(IReadOnlyList<Monster> monsters)
        {
            foreach (var hero in Heroes.Values)
            {
                if (hero.TargetedMonster != null)
                {
                    if (monsters.All(x => x.Id != hero.TargetedMonster.Id))
                    {
                        Io.Debug($"removing {hero.TargetedMonster.Id} from {hero.Id}");
                        hero.TargetedMonster = null;
                    }
                    else
                    {
                        var monster = monsters.Single(x => x.Id == hero.TargetedMonster.Id);

                        if (monster.ThreatFor != 1)
                        {
                            Io.Debug($"removing {hero.TargetedMonster.Id} from {hero.Id} because its not threat for base anymore");
                            hero.TargetedMonster = null;
                        }
                    }
                }
            }
        }
    }
}
