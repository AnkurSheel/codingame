namespace SpringChallenge2021.Actions
{
    public class SeedAction : IAction
    {
        private readonly int _srcTreeIndex;
        public int SeedIndex { get; }

        public SeedAction(int srcTreeIndex, int seedIndex)
        {
            _srcTreeIndex = srcTreeIndex;
            SeedIndex = seedIndex;
        }

        public string GetOutputAction()
        {
            return $"SEED {_srcTreeIndex} {SeedIndex}";
        }
    }
}