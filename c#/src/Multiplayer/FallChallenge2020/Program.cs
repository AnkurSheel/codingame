using System.Collections.Generic;

using FallChallenge2020.Models;
using FallChallenge2020.Services;

namespace FallChallenge2020
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string[] inputs;

            // game loop
            while (true)
            {
                var numberOfClientOrders = int.Parse(Io.ReadLine()); // the number of spells and recipes in play
                var availableClientOrders = new List<GameAction>();

                for (var i = 0; i < numberOfClientOrders; i++)
                {
                    inputs = Io.ReadLine().Split(' ');

                    var gameAction = new GameAction
                                     {
                                         ActionId = int.Parse(inputs[0]),
                                         ActionType = ActionTypeExtension.FromString(inputs[1]),
                                         IngredientsCost = new Ingredient
                                                           {
                                                               Tier0 = int.Parse(inputs[2]),
                                                               Tier1 = int.Parse(inputs[3]),
                                                               Tier2 = int.Parse(inputs[4]),
                                                               Tier3 = int.Parse(inputs[5])
                                                           },
                                         Price = int.Parse(inputs[6]) // the price in rupees if this is a potion
                                     };
                    availableClientOrders.Add(gameAction);
                }

                //foreach (var clientOrder in availableClientOrders)
                //{
                //    Io.Debug(clientOrder.ToString());
                //}
                inputs = Io.ReadLine().Split(' ');
                var myPlayer = new Player
                               {
                                   Inventory = new Ingredient
                                               {
                                                   Tier0 = int.Parse(inputs[0]), // tier-0 ingredients in inventory
                                                   Tier1 = int.Parse(inputs[1]),
                                                   Tier2 = int.Parse(inputs[2]),
                                                   Tier3 = int.Parse(inputs[3])
                                               },
                                   Score = int.Parse(inputs[4]) // amount of rupees
                               };

                inputs = Io.ReadLine().Split(' ');
                var opponentPlayer = new Player
                                     {
                                         Inventory = new Ingredient
                                                     {
                                                         Tier0 = int.Parse(inputs[0]), // tier-0 ingredients in inventory
                                                         Tier1 = int.Parse(inputs[1]),
                                                         Tier2 = int.Parse(inputs[2]),
                                                         Tier3 = int.Parse(inputs[3])
                                                     },
                                         Score = int.Parse(inputs[4]) // amount of rupees
                                     };

                var action = "WAIT";
                foreach (var clientOrder in availableClientOrders)
                {
                    if (clientOrder.IngredientsCost.Tier0 < 0 && myPlayer.Inventory.Tier0 > -clientOrder.IngredientsCost.Tier0)
                    {
                        action = $"BREW {clientOrder.ActionId}";
                        break;
                    }

                    if (clientOrder.IngredientsCost.Tier1 < 0 && myPlayer.Inventory.Tier1 > -clientOrder.IngredientsCost.Tier1)
                    {
                        action = $"BREW {clientOrder.ActionId}";
                        break;
                    }

                    if (clientOrder.IngredientsCost.Tier2 < 0 && myPlayer.Inventory.Tier2 > -clientOrder.IngredientsCost.Tier2)
                    {
                        action = $"BREW {clientOrder.ActionId}";
                        break;
                    }

                    if (clientOrder.IngredientsCost.Tier3 < 0 && myPlayer.Inventory.Tier3 > -clientOrder.IngredientsCost.Tier3)
                    {
                        action = $"BREW {clientOrder.ActionId}";
                        break;
                    }
                }

                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");

                // in the first league: BREW <id> | WAIT; later: BREW <id> | CAST <id> [<times>] | LEARN <id> | REST | WAIT
                Io.WriteLine(action);
            }
        }
    }
}
