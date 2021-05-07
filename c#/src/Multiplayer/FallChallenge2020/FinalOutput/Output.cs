using System.Collections.Generic;
using FallChallenge2020.Agents;
using FallChallenge2020.Common;
using FallChallenge2020.Models;
using System;
using System.Text;
using System.Linq;
using FallChallenge2020.Actions;
using System.IO;


 // 14/11/2020 12:56

namespace FallChallenge2020
{
    public class Constants
    {
        public const int IngredientTiers = 4;
    }
}


namespace FallChallenge2020
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Io.Initialize();
            var agent = new SimpleAgent();
            // game loop
            while (true)
            {
                var game = new Game ();
                game.Initialize();
                var action = agent.GetAction(game);

                Io.WriteLine(action.GetAction());
            }
        }

        
    }
}
namespace FallChallenge2020.Actions
{
    public class BrewAction : IAction
    {
        private readonly int _actionId;

        public BrewAction(int actionId)
        {
            _actionId = actionId;
        }

        public string GetAction()
        {
            return $"BREW {_actionId}";
        }
    }
}
namespace FallChallenge2020.Actions
{
    public class CastAction : IAction
    {
        private readonly int _actionId;

        public CastAction(int actionId)
        {
            _actionId = actionId;
        }

        public string GetAction()
        {
            return $"CAST {_actionId}";
        }
    }
}
namespace FallChallenge2020.Actions
{
    public interface IAction
    {
        string GetAction();
    }
}
namespace FallChallenge2020.Actions
{
    public class RestAction : IAction
    {
        public string GetAction()
        {
            return "REST";
        }
    }
}

namespace FallChallenge2020.Actions
{
    public class WaitAction : IAction
    {
        public string GetAction()
        {
            return "WAIT";
        }
    }
}


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
namespace FallChallenge2020.Common
{
    public static class Constants
    {
        public static readonly bool IsDebugOn = true;

        public static readonly bool IsForInput = false;

        public static readonly bool IsLocalRun = false;
    }
}

namespace FallChallenge2020.Common
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

namespace FallChallenge2020.Models
{
    public enum ActionType
    {
        Unknown,

        Brew,

        Cast,

        OpponentCast
    }

    public static class ActionTypeExtension
    {
        public static ActionType FromString(string actionType)
        {
            if (actionType == "BREW")
            {
                return ActionType.Brew;
            }

            if (actionType == "CAST")
            {
                return ActionType.Cast;
            }

            if (actionType == "OPPONENT_CAST")
            {
                return ActionType.OpponentCast;
            }

            throw new ArgumentOutOfRangeException($"Passed {actionType} for action type");
        }
    }
}


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
namespace FallChallenge2020.Models
{
    internal class Player
    {
        public int[] Inventory { get; set; }

        public int Score { get; set; }
    }
}
namespace FallChallenge2020.Models
{
    internal class Potion
    {
        public Potion(
            int actionId,
            int tier0IngredientCost,
            int tier1IngredientCost,
            int tier2IngredientCost,
            int tier3IngredientCost,
            int price)
        {
            ActionId = actionId;
            Price = price;
            IngredientsCost = new[] { -tier0IngredientCost, -tier1IngredientCost, -tier2IngredientCost, -tier3IngredientCost };
        }

        public int ActionId { get; }

        public int Price { get; }

        public int[] IngredientsCost { get; }

        public override string ToString()
        {
            return $"Potion {ActionId} {IngredientsCost[0]} {IngredientsCost[1]} {IngredientsCost[2]} {IngredientsCost[3]} {Price}";
        }
    }
}
namespace FallChallenge2020.Models
{
    internal class Spell
    {
        public Spell(
            int actionId,
            int tier0IngredientCost,
            int tier1IngredientCost,
            int tier2IngredientCost,
            int tier3IngredientCost,
            bool castable)
        {
            ActionId = actionId;
            Castable = castable;
            IngredientsCost = new[] { tier0IngredientCost, tier1IngredientCost, tier2IngredientCost, tier3IngredientCost };
        }

        public int ActionId { get; }

        public bool Castable { get; }

        public int[] IngredientsCost { get; }

        public override string ToString()
        {
            return $"Spell {ActionId} {IngredientsCost[0]} {IngredientsCost[1]} {IngredientsCost[2]} {IngredientsCost[3]} {Castable}";
        }
    }
}