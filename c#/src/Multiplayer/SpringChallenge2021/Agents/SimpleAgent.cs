using System.Linq;
using SpringChallenge2021.Actions;
using SpringChallenge2021.Models;

namespace SpringChallenge2021.Agents
{
    public class SimpleAgent
    {
        public IAction GetAction(Game game)
        {
            var completeAction = GetBestCompleteAction(game);
            if (completeAction != null)
            {
                return completeAction;
            }

            var growAction = GetBestGrowAction(game);
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

        private static IAction? GetBestCompleteAction(Game game)
        {
            if (game.MyPlayer.Trees[TreeSize.Large].Count < 3 && game.Day < 22)
            {
                return null;
            }

            var completeActions = game.PossibleActions.OfType<CompleteAction>().ToList();
            var bestCompleteAction = completeActions.FirstOrDefault();
            var bestSoilQuality = SoilQuality.Unusable;
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

        private static IAction? GetBestGrowAction(Game game)
        {
            var growActions = game.PossibleActions.OfType<GrowAction>().ToList();
            var bestSoilQuality = SoilQuality.Unusable;
            var bestTreeSize = TreeSize.Seed;
            var bestGrowAction = growActions.FirstOrDefault();
            foreach (var growAction in growActions)
            {
                var cellTreeSize = game.Trees[growAction.Index].Size;

                if (bestTreeSize < cellTreeSize)
                {
                    bestTreeSize = cellTreeSize;
                    bestGrowAction = growAction;
                    bestSoilQuality = game.Board[growAction.Index].SoilQuality;
                }
                else if (bestTreeSize == cellTreeSize)
                {
                    var cellSoilQuality = game.Board[growAction.Index].SoilQuality;
                    if (bestSoilQuality < cellSoilQuality)
                    {
                        bestSoilQuality = cellSoilQuality;
                        bestGrowAction = growAction;
                    }
                }
            }

            return bestGrowAction;
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
