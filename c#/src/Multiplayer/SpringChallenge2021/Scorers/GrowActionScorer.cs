using System.Linq;
using SpringChallenge2021.Actions;
using SpringChallenge2021.Models;

namespace SpringChallenge2021.Scorers
{
    public class GrowActionScorer
    {
        public IAction? GetBestGrowAction(Game game)
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
    }
}