using System.Collections.Generic;

using FallChallenge2020.Common;

namespace FallChallenge2020.Models
{
    internal class Game
    {
        public List<Potion> Potions { get; private set; }

        public List<Spell> Spells { get; private set; }

        public List<Spell> OpponentSpells { get; private set; }

        public Player[] Players { get; private set; }

        public Player MyPlayer => Players[0];

        public void Initialize()
        {
            Potions = new List<Potion>();
            Spells = new List<Spell>();
            OpponentSpells = new List<Spell>();
            ParseActions();
            Players = new[] { GetPlayer(), GetPlayer() };
        }

        private void ParseActions()
        {
            var numberOfActions = int.Parse(Io.ReadLine());

            for (var i = 0; i < numberOfActions; i++)
            {
                var inputs = Io.ReadLine().Split(' ');

                var actionType = ActionTypeExtension.FromString(inputs[1]);
                if (actionType == ActionType.Brew)
                {
                    Potions.Add(new Potion(int.Parse(inputs[0]),
                                           int.Parse(inputs[2]),
                                           int.Parse(inputs[3]),
                                           int.Parse(inputs[4]),
                                           int.Parse(inputs[5]),
                                           int.Parse(inputs[6])));
                }
                else if (actionType == ActionType.Cast)
                {
                    Spells.Add(new Spell(int.Parse(inputs[0]),
                                         int.Parse(inputs[2]),
                                         int.Parse(inputs[3]),
                                         int.Parse(inputs[4]),
                                         int.Parse(inputs[5]),
                                         int.Parse(inputs[9]) != 0));
                }
                else if (actionType == ActionType.OpponentCast)
                {
                    OpponentSpells.Add(new Spell(int.Parse(inputs[0]),
                                                 int.Parse(inputs[2]),
                                                 int.Parse(inputs[3]),
                                                 int.Parse(inputs[4]),
                                                 int.Parse(inputs[5]),
                                                 int.Parse(inputs[9]) != 0));
                }
            }
        }

        private Player GetPlayer()
        {
            var inputs = Io.ReadLine().Split(' ');
            var myPlayer = new Player
                           {
                               Inventory = new[] { int.Parse(inputs[0]), int.Parse(inputs[1]), int.Parse(inputs[2]), int.Parse(inputs[3]) },
                               Score = int.Parse(inputs[4])
                           };
            return myPlayer;
        }
    }
}
