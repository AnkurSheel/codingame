using FallChallenge2020.Actions;
using FallChallenge2020.Models;

namespace FallChallenge2020.Agents
{
    internal class SimpleAgent
    {
        public IAction GetAction(Game game)
        {
            var inventory = game.MyPlayer.Inventory;
            foreach (var potion in game.Potions)
            {
                var ingredientsCost = potion.IngredientsCost;
                for (var i = 0; i < 4; i++)
                {
                    if (ingredientsCost[i] < 0 && inventory[i] > -ingredientsCost[i])
                    {
                        return new BrewAction(potion.ActionId);
                    }
                }
            }

            return new WaitAction();
        }
    }
}
