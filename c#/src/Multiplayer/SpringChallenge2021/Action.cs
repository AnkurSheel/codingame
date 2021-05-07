using SpringChallenge2021.Actions;

namespace SpringChallenge2021
{
    internal class Action
    {
        private const string SEED = "SEED";
        private const string GROW = "GROW";
        private const string COMPLETE = "COMPLETE";

        public static IAction Parse(string action)
        {
            var parts = action.Split(" ");
            return parts[0] switch
            {
                SEED => new SeedAction(int.Parse(parts[1]), int.Parse(parts[2])),
                GROW => new GrowAction(int.Parse(parts[1])),
                COMPLETE => new CompleteAction(int.Parse(parts[1])),
                _ => new WaitAction()
            };
        }
    }
}
