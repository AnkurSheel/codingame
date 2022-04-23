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
            var growActions = game.PossibleActions.OfType<GrowAction>().ToList();
            if (!growActions.Any()
                || game.MyPlayer.Trees.Count >= Constants.MaxTreesToKeep
                || game.Day >= Constants.DayCutOffForHarvesting && game.MyPlayer.Trees[TreeSize.Large].Any())
            {
                return null;
            }

            var costForGrowActions = GetCostForAction(game.MyPlayer.Trees);

            var bestScore = int.MinValue;
            GrowAction bestGrowAction = null;
            foreach (var growAction in growActions)
            {
                var cell = game.Board[growAction.Index];
                var tree = game.Trees[growAction.Index];
                var cellTreeSize = tree.Size;
                var sizeOfTreeAfterGrowth = cellTreeSize + 1;

                if (game.Day >= Constants.DayCutOffForGrowing && sizeOfTreeAfterGrowth != TreeSize.Large)
                {
                    continue;
                }

                if (game.ShadowsNextDay.ContainsKey(cell))
                {
                    var sizeOfTreeCastingShadow = game.ShadowsNextDay[cell];
                    if (sizeOfTreeAfterGrowth <= sizeOfTreeCastingShadow.Size)
                    {
                        continue;
                    }
                }

                var numberOfOpponentTreesBlocked =
                    game.Shadows.Count(x =>
                        x.Value == tree && !x.Value.IsMine && x.Value.Size <= sizeOfTreeAfterGrowth);
                var numberOfMyTreesBlocked =
                    game.Shadows.Count(x => x.Value == tree && x.Value.IsMine && x.Value.Size <= sizeOfTreeAfterGrowth);
                var score = (int) sizeOfTreeAfterGrowth
                            - costForGrowActions[sizeOfTreeAfterGrowth]
                            + (int) cell.SoilQuality;

                if (sizeOfTreeAfterGrowth == TreeSize.Large)
                {
                    score += game.Nutrients;
                }

                Io.Debug(
                    $"Grow Action Score - Score:{score} - Index:{growAction.Index} - sizeOfTreeAfterGrowth:{sizeOfTreeAfterGrowth} - costForGrowActions:{costForGrowActions[sizeOfTreeAfterGrowth]} - soilQuality:{cell.SoilQuality} - numberOfOpponentTreesBlocked:{numberOfOpponentTreesBlocked} - numberOfMyTreesBlocked:{numberOfMyTreesBlocked}");

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
