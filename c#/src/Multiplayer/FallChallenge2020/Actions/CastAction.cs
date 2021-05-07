namespace FallChallenge2020.Actions
{
    public class CastAction : IAction
    {
        private readonly int _actionId;

        public CastAction(int actionId)
        {
            _actionId = actionId;
        }

        public string GetAction()
        {
            return $"CAST {_actionId}";
        }
    }
}
