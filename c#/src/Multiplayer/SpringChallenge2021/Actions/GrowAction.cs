namespace SpringChallenge2021.Actions
{
    public class GrowAction : IAction
    {
        public int Index { get; }

        public GrowAction(int index)
        {
            Index = index;
        }

        public string GetOutputAction()
        {
            return $"GROW {Index}";
        }
    }
}