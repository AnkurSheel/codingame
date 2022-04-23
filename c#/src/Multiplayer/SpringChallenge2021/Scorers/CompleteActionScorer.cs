using System.Linq;
using SpringChallenge2021.Actions;
using SpringChallenge2021.Models;

namespace SpringChallenge2021.Scorers
{
    public class CompleteActionScorer
    {
        private readonly ShadowScorer _shadowScorer;

        public CompleteActionScorer()
        {
            _shadowScorer = new ShadowScorer();
        }

        public IAction? GetBestCompleteAction(Game game)
        {
            var completeActions = game.PossibleActions.OfType<CompleteAction>().ToList();

            if (!completeActions.Any() || game.MyPlayer.Trees[TreeSize.Large].Count < Constants.MaxLargeTreesToKeep && game.Day < Constants.DayCutOffForHarvesting)
            {
                return null;
            }

            CompleteAction bestCompleteAction = null;
            var bestScore = -1;
            foreach (var completeAction in completeActions)
            {
                var score = (int) game.Board[completeAction.Index].SoilQuality;
                if (bestScore < score)
                {
                    bestScore = score;
                    bestCompleteAction = completeAction;
                }
            }

            return bestCompleteAction;
        }
    }
}
