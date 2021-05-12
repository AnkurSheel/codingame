using System.Collections.Generic;
using System.Linq;
using SpringChallenge2021.Actions;
using SpringChallenge2021.Common.Services;
using SpringChallenge2021.Models;

namespace SpringChallenge2021.Scorers
{
    public class GrowActionScorer
    {
        private static readonly Dictionary<TreeSize, int> _treeGrowthBaseCost = new Dictionary<TreeSize, int>
        {
            {TreeSize.Small, 1},
            {TreeSize.Medium, 3},
            {TreeSize.Large, 7}
        };

        public IAction? GetBestGrowAction(Game game)
        {
            var costForGrowActions = GetCostForAction(game.MyPlayer.Trees);

            var growActions = game.PossibleActions.OfType<GrowAction>().ToList();
            var bestScore = int.MinValue;
            var bestGrowAction = growActions.FirstOrDefault();
            foreach (var growAction in growActions)
            {
                var tree = game.Trees[growAction.Index];
                var cellTreeSize = tree.Size;
                var sizeOfTreeAfterGrowth = cellTreeSize + 1;

                var cell = game.Board[growAction.Index];
                if (game.ShadowsNextDay.ContainsKey(cell))
                {
                    var sizeOfTreeCastingShadow = game.ShadowsNextDay[cell];
                    if (sizeOfTreeAfterGrowth <= sizeOfTreeCastingShadow)
                    {
                        continue;
                    }
                }

                var score = (int) sizeOfTreeAfterGrowth
                            - costForGrowActions[sizeOfTreeAfterGrowth]
                            + (int) cell.SoilQuality;

                if (sizeOfTreeAfterGrowth == TreeSize.Large)
                {
                    score += game.Nutrients;
                }

                if (bestScore < score)
                {
                    bestScore = score;
                    bestGrowAction = growAction;
                }
            }

            return bestGrowAction;
        }

        private Dictionary<TreeSize, int> GetCostForAction(Dictionary<TreeSize, List<Tree>> trees)
        {
            var costOfGrowActions = new Dictionary<TreeSize, int>();
            var treeSizes = EnumHelpers.GetAllValues<TreeSize>().ToList().Except(new[] {TreeSize.Seed});
            foreach (var treeSize in treeSizes)
            {
                var numberOfTrees = trees[treeSize].Count;
                Io.Debug($"Number of trees of size {treeSize} = {numberOfTrees}");
                costOfGrowActions.Add(treeSize, numberOfTrees + _treeGrowthBaseCost[treeSize]);
            }

            return costOfGrowActions;
        }
    }
}
