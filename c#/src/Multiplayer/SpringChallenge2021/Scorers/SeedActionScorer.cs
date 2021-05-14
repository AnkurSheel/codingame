using System.Linq;
using SpringChallenge2021.Actions;
using SpringChallenge2021.Models;

namespace SpringChallenge2021.Agents
{
    public class SeedActionScorer
    {
        public IAction? GetBestSeedAction(Game game)
        {
            var seedActions = game.PossibleActions.OfType<SeedAction>().ToList();
            if (!seedActions.Any() || game.MyPlayer.Trees.Count >= Constants.MaxTreesToKeep || game.Day >= Constants.DayCutOffForGrowing)
            {
                return null;
            }

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
