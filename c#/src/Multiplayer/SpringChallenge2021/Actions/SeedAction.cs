namespace SpringChallenge2021.Actions
{
    public class SeedAction : IAction
    {
        private readonly int _srcTreeIndex;
        private readonly int _seedIndex;

        public SeedAction(int srcTreeIndex, int seedIndex)
        {
            _srcTreeIndex = srcTreeIndex;
            _seedIndex = seedIndex;
        }

        public string GetOutputAction()
        {
            return $"SEED {_srcTreeIndex} {_seedIndex}";
        }
    }
}
