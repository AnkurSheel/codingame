using System.Collections.Generic;
using System.Linq;

using FallChallenge2020.Actions;
using FallChallenge2020.Common;
using FallChallenge2020.Models;

namespace FallChallenge2020.Agents
{
    internal class SimpleAgent
    {
        public IAction GetAction(Game game)
        {
            var bestPotion = GetBestPotion(game);

            var inventory = game.MyPlayer.Inventory;

            foreach (var spell in game.Spells)
            {
                Io.Debug(spell.ToString());
            }

            Io.Debug($"Building {bestPotion}");

            var availableSpells = game.Spells.Where(s => s.Castable).ToList();
            var nextSpell = NextSpell(bestPotion.IngredientsCost, inventory, availableSpells);
            if (nextSpell != null)
            {
                return nextSpell;
            }

            return new BrewAction(bestPotion.ActionId);
        }

        private Potion GetBestPotion(Game game)
        {
            Potion bestPotion = null;
            var bestWeightedPrice = int.MinValue;
            foreach (var potion in game.Potions)
            {
                var penalty = 0;
                for (var i = 0; i < Constants.IngredientTiers; i++)
                {
                    penalty += i * potion.IngredientsCost[i];
                }

                var weightedPrice = potion.Price - penalty;
                Io.Debug($"{weightedPrice} {potion}");
                if (weightedPrice > bestWeightedPrice)
                {
                    bestPotion = potion;
                    bestWeightedPrice = weightedPrice;
                }
            }

            Io.Debug($"Best weighted price {bestWeightedPrice} {bestPotion}");
            return bestPotion;
        }

        private IAction GetNextSpell(Spell spell, int[] inventory, List<Spell> spells)
        {
            Io.Debug($"Checking spell {spell}");
            var nextSpell = NextSpell(spell.IngredientsCost, inventory, spells);
            if (nextSpell != null)
            {
                return nextSpell;
            }

            return new CastAction(spell.ActionId);
        }

        private IAction NextSpell(int[] ingredientsCost, int[] inventory, List<Spell> spells)
        {
            var needsReset = false;
            for (var i = 0; i < Constants.IngredientTiers; i++)
            {
                if (ingredientsCost[i] < 0)
                {
                    Io.Debug($"Needs {-ingredientsCost[i]} of Tier{i}");
                    if (inventory[i] > 0)
                    {
                        Io.Debug($"Has {inventory[i]} of Tier{i}");
                    }

                    if (inventory[i] < -ingredientsCost[i])
                    {
                        Io.Debug($"Needs spell for Tier{i}");
                        var requiredSpell = spells.FirstOrDefault(s => s.IngredientsCost[i] > 0);
                        if (requiredSpell == null)
                        {
                            needsReset = true;
                        }
                        else
                        {
                            return GetNextSpell(requiredSpell, inventory, spells);
                        }
                    }
                }
            }

            if (needsReset)
            {
                return new RestAction();
            }

            return null;
        }
    }
}
