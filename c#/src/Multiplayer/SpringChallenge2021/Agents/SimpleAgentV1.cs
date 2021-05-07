using System.Linq;
using SpringChallenge2021.Actions;

namespace SpringChallenge2021.Agents
{
    public class SimpleAgentV1
    {
        public IAction GetAction(Game game)
        {
            var completeAction = game.PossibleActions.FirstOrDefault(x => x is CompleteAction);
            if (completeAction != null)
            {
                return completeAction;
            }

            var growAction = game.PossibleActions.FirstOrDefault(x => x is GrowAction);
            if (growAction != null)
            {
                return growAction;
            }

            var seedAction = game.PossibleActions.FirstOrDefault(x => x is SeedAction);
            if (seedAction != null)
            {
                return seedAction;
            }

            return new WaitAction();
        }
    }
}
