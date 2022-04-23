using SpringChallenge2022.Agents;

namespace SpringChallenge2022.Actions
{
    public class WindSpellAction : IAction
    {
        private const int Range = 1280;
        private readonly Vector _targetPosition;

        public WindSpellAction(Vector targetPosition)
        {
            _targetPosition = targetPosition;
        }

        public string GetOutputAction()
            => $"SPELL WIND {_targetPosition.X} {_targetPosition.Y}";
    }
}
