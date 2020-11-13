using System;
using System.Net;

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