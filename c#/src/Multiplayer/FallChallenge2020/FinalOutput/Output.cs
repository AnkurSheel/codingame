using System.Collections.Generic;
using FallChallenge2020.Models;
using FallChallenge2020.Services;
using System;
using System.Net;
using System.IO;


 // 13/11/2020 09:41

namespace FallChallenge2020
{
    public static class Constants
    {
        public static readonly bool IsDebugOn = true;

        public static readonly bool IsForInput = false;

        public static readonly bool IsLocalRun = false;
    }
}


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

namespace FallChallenge2020.Models
{
    internal class GameAction
    {
        public int ActionId { get; set; }

        public ActionType ActionType { get; set; }

        public int Price { get; set; }

        public Ingredient IngredientsCost { get; set; }

        public override string ToString()
        {
            return $"{ActionId} {ActionType} {IngredientsCost} {Price}"; }
    }

    public enum ActionType
    {
        Unknown,
        Brew,
    }

    public static class ActionTypeExtension
    {
        public static ActionType FromString(string actionType)
        {
            if (actionType == "BREW")
            {
                return ActionType.Brew;
            }
            throw new ArgumentOutOfRangeException($"Passed {actionType} for action type");
        }
    }
}
namespace FallChallenge2020.Models
{
    internal class Ingredient
    {
        public int Tier0 { get; set; }

        public int Tier1 { get; set; }

        public int Tier2 { get; set; }

        public int Tier3 { get; set; }
    }
}
namespace FallChallenge2020.Models
{
    internal class Player
    {
        public Ingredient Inventory { get; set; }

        public int Score { get; set; }
    }
}

namespace FallChallenge2020.Services
{
    public static class Io
    {
        private static StreamReader _file;

        public static void Initialize()
        {
            if (Constants.IsLocalRun)
            {
                _file = new StreamReader(@".\in.txt");
            }
        }

        public static void Debug(string output)
        {
            if (Constants.IsDebugOn || Constants.IsForInput)
            {
                Console.Error.WriteLine(output);
            }
        }

        public static void WriteLine(string output)
        {
            Console.WriteLine(output);
        }

        public static string ReadLine()
        {
            if (Constants.IsLocalRun)
            {
                return _file.ReadLine();
            }
            else
            {
                var input = Console.ReadLine();

                if (Constants.IsForInput)
                {
                    Debug("IN");
                    Debug(input);
                    Debug("/IN");
                }
                else
                {
                    Debug(input);
                }

                return input;
            }
        }
    }
}