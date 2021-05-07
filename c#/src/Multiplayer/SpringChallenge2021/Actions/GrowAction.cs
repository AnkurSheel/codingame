namespace SpringChallenge2021.Actions
{
    public class GrowAction : IAction
    {
        private readonly int _index;

        public GrowAction(int index)
        {
            _index = index;
        }

        public string GetOutputAction()
        {
            return $"GROW {_index}";
        }
    }
}
