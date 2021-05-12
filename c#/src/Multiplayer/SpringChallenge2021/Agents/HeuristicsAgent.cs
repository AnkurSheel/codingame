using System.Linq;
using SpringChallenge2021.Actions;
using SpringChallenge2021.Models;
using SpringChallenge2021.Scorers;

namespace SpringChallenge2021.Agents
{
    public class HeuristicsAgent
    {
        private readonly GrowActionScorer _growActionScorer;

        public HeuristicsAgent()
        {
            _growActionScorer = new GrowActionScorer();
        }

        public IAction GetAction(Game game)
        {
            var completeAction = GetBestCompleteAction(game);
            if (completeAction != null)
            {
                return completeAction;
            }

            var growAction = _growActionScorer.GetBestGrowAction(game);
            if (growAction != null)
            {
                return growAction;
            }

            if (game.Day <= Constants.DayCutOffForGrowing)
            {
                var seedAction = GetBestSeedAction(game);
                if (seedAction != null)
                {
                    return seedAction;
                }
            }

            return new WaitAction();
        }

        private static IAction? GetBestCompleteAction(Game game)
        {
            var completeActions = game.PossibleActions.OfType<CompleteAction>().ToList();

            if (!completeActions.Any()
                || game.MyPlayer.Trees[TreeSize.Large].Count < Constants.MaxLargeTreesToKeep
                && game.Day < Constants.DayCutOffForHarvesting)
            {
                return null;
            }

            var bestCompleteAction = completeActions.FirstOrDefault();
            var bestSoilQuality = game.Board[bestCompleteAction.Index].SoilQuality;
            foreach (var completeAction in completeActions)
            {
                var cellSoilQuality = game.Board[completeAction.Index].SoilQuality;
                if (bestSoilQuality < cellSoilQuality)
                {
                    bestSoilQuality = cellSoilQuality;
                    bestCompleteAction = completeAction;
                }
            }

            return bestCompleteAction;
        }

        private IAction? GetBestSeedAction(Game game)
        {
            if (game.MyPlayer.Trees[TreeSize.Seed].Any() || game.MyPlayer.Trees.Count >= Constants.MaxTreesToKeep)
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
