using SpringChallenge2021.Actions;
using SpringChallenge2021.Scorers;

namespace SpringChallenge2021.Agents
{
    public class HeuristicsAgent
    {
        private readonly GrowActionScorer _growActionScorer;
        private readonly CompleteActionScorer _completeActionScorer;
        private readonly SeedActionScorer _seedActionScorer;

        public HeuristicsAgent()
        {
            _growActionScorer = new GrowActionScorer();
            _completeActionScorer = new CompleteActionScorer();
            _seedActionScorer = new SeedActionScorer();
        }

        public IAction GetAction(Game game)
        {
            var completeAction = _completeActionScorer.GetBestCompleteAction(game);
            if (completeAction != null)
            {
                return completeAction;
            }

            var growAction = _growActionScorer.GetBestGrowAction(game);
            if (growAction != null)
            {
                return growAction;
            }

            var seedAction = _seedActionScorer.GetBestSeedAction(game);
            if (seedAction != null)
            {
                return seedAction;
            }

            return new WaitAction();
        }
    }
}
