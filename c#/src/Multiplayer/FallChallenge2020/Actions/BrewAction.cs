namespace FallChallenge2020.Actions
{
    public class BrewAction : IAction
    {
        private readonly int _actionId;

        public BrewAction(int actionId)
        {
            _actionId = actionId;
        }

        public string GetAction()
        {
            return $"BREW {_actionId}";
        }
    }
}
