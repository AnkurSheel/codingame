using System;

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
