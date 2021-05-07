using System.Linq;
using SpringChallenge2021.Actions;

namespace SpringChallenge2021.Agents
{
    public class SimpleAgent
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

            var seedAction = GetBestSeedAction(game);
            if (seedAction != null)
            {
                return seedAction;
            }

            return new WaitAction();
        }

        private IAction? GetBestSeedAction(Game game)
        {
            if (game.MyPlayer.Trees[TreeSize.Seed].Any())
            {
                return null;
            }

            var seedActions = game.PossibleActions.OfType<SeedAction>().ToList();
            var bestSoilQuality = SoilQuality.Unusable;
            var bestSeedAction = seedActions.FirstOrDefault();
            foreach (var seedAction in seedActions)
            {
                var cellSoilQuality = game.Board[seedAction.SeedIndex].SoilQuality;
                if (bestSoilQuality < cellSoilQuality)
                {
                    bestSoilQuality = cellSoilQuality;
                    bestSeedAction = seedAction;
                }
            }

            return bestSeedAction;
        }
    }
}
