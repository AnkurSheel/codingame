using System.Collections.Generic;
using System.Linq;
using SpringChallenge2021.Models;

namespace SpringChallenge2021.Scorers
{
    public class ShadowScorer
    {
        public int GetScoreFromOpponentTreesInShadow(Tree tree, TreeSize sizeOfTreeAfterGrowth, IReadOnlyDictionary<Cell, Tree> shadows)
        {
            var opponentTreesBlocked = shadows.Where(x => x.Value == tree && !x.Value.IsMine && x.Value.Size <= sizeOfTreeAfterGrowth);

            var opponentScore = opponentTreesBlocked.Sum(opponentTree => (int) opponentTree.Value.Size);

            return opponentScore;
        }

        public int GetScoreFromMyTreesInShadow(Tree tree, TreeSize sizeOfTreeAfterGrowth, IReadOnlyDictionary<Cell, Tree> shadows)
        {
            var myTreesBlocked = shadows.Where(x => x.Value == tree && x.Value.IsMine && x.Value.Size <= sizeOfTreeAfterGrowth);

            var myScore = myTreesBlocked.Sum(myTree => (int) myTree.Value.Size);
            return myScore;
        }
    }
}
