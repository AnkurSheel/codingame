using System.Linq;
using SpringChallenge2021.Actions;
using SpringChallenge2021.Common.Services;
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

            if (game.Day < Constants.DayCutOff + 1)
            {
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
            }

            return new WaitAction();
        }

        private static IAction? GetBestCompleteAction(Game game)
        {
            var completeActions = game.PossibleActions.OfType<CompleteAction>().ToList();

            if (!completeActions.Any()
                || game.MyPlayer.Trees[TreeSize.Large].Count < Constants.MaxLargeTreesToKeep
                && game.Day < Constants.DayCutOff)
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

        private static IAction? GetBestGrowAction(Game game)
        {
            var growActions = game.PossibleActions.OfType<GrowAction>().ToList();
            var bestSoilQuality = SoilQuality.Unusable;
            var bestTreeSize = TreeSize.Seed;
            var bestGrowAction = growActions.FirstOrDefault();
            foreach (var growAction in growActions)
            {
                var cell = game.Board[growAction.Index];
                if (game.ShadowsNextDay.ContainsKey(cell))
                {
                    var sizeOfTreeCastingShadow = game.ShadowsNextDay[cell];
                    var sizeOfTreeAfterGrowth = game.Trees[growAction.Index].Size + 1;
                    if (sizeOfTreeAfterGrowth <= sizeOfTreeCastingShadow)
                    {
                        continue;
                    }
                }

                var cellTreeSize = game.Trees[growAction.Index].Size;

                if (bestTreeSize < cellTreeSize)
                {
                    bestTreeSize = cellTreeSize;
                    bestGrowAction = growAction;
                    bestSoilQuality = cell.SoilQuality;
                }
                else if (bestTreeSize == cellTreeSize)
                {
                    var cellSoilQuality = cell.SoilQuality;
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
