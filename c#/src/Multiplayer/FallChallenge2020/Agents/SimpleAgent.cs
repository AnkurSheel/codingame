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
            var potion = game.Potions[0];
            var inventory = game.MyPlayer.Inventory;

            foreach (var spell in game.Spells)
            {
                Io.Debug(spell.ToString());
            }

            Io.Debug($"Building potion {potion}");

            var ingredientsCost = potion.IngredientsCost;
            var needsReset = false;
            for (var i = 0; i < Constants.IngredientTiers; i++)
            {
                if (ingredientsCost[i] > 0)
                {
                    Io.Debug($"Needs {ingredientsCost[i]} of Tier{i}");
                    if (inventory[i] > 0)
                    {
                        Io.Debug($"Has {inventory[i]} of Tier{i}");
                    }

                    if (inventory[i] < ingredientsCost[i])
                    {
                        Io.Debug($"Needs spell for Tier{i}");
                        var availableSpells = game.Spells.Where(s => s.Castable).ToList();
                        var requiredSpell = availableSpells.FirstOrDefault(s => s.IngredientsCost[i] > 0);
                        if (requiredSpell == null)
                        {
                            needsReset = true;
                        }

                        else
                        {
                            var spellAction = GetNextSpell(requiredSpell, inventory, availableSpells);
                            Io.Debug($"Got spell {spellAction.GetAction()}");
                            return spellAction;
                        }
                    }
                }
            }

            if (needsReset)
            {
                return new RestAction();
            }

            return new BrewAction(potion.ActionId);
        }

        private IAction GetNextSpell(Spell spell, int[] inventory, List<Spell> spells)
        {
            Io.Debug($"Checking spell {spell}");
            var ingredientsCost = spell.IngredientsCost;
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

            return new CastAction(spell.ActionId);
        }
    }
}
