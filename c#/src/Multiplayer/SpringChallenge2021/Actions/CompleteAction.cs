namespace SpringChallenge2021.Actions
{
    public class CompleteAction : IAction
    {
        private readonly int _index;

        public CompleteAction(int index)
        {
            _index = index;
        }

        public string GetOutputAction()
        {
            return $"COMPLETE {_index}";
        }
    }
}
