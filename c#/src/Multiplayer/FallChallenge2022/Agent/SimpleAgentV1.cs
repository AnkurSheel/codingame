using System.Collections.Generic;
using FallChallenge2022.Action;
using FallChallenge2022.Common.Services;
using SpringChallenge2021.Actions;

namespace FallChallenge2022.Agent
{
    public class SimpleAgentV1 : IAgent
    {
        public IReadOnlyList<IAction> GetActions(Game game)
        {
            var actions = new List<IAction>();
            Io.Debug(game.Width.ToString());
            Io.Debug(game.Height.ToString());
            
            foreach (var unit in game.MyPlayer.Units)
            {
                var nextX = Constants.RandomGenerator.Next(game.Width - 1);
                var nextY = Constants.RandomGenerator.Next(game.Height - 1);
                actions.Add(new MoveAction(unit.PosX, unit.PosY, nextX, nextY));
            }

            if (actions.Count == 0)
            {
                actions.Add(new WaitAction());
            }
            
            return actions;
        }
    }
}
