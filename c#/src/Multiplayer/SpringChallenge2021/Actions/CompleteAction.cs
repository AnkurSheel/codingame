namespace SpringChallenge2021.Actions
{
    public class CompleteAction : IAction
    {
        public int Index { get; }

        public CompleteAction(int index)
        {
            Index = index;
        }

        public string GetOutputAction()
        {
            return $"COMPLETE {Index}";
        }
    }
}
