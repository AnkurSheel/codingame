using System.Numerics;

namespace SpringChallenge2022.Actions
{
    public class WindSpellAction : IAction
    {
        private const int Range = 1280;
        private readonly Vector2 _targetPosition;

        public WindSpellAction(Vector2 targetPosition)
        {
            _targetPosition = targetPosition;
        }

        public string GetOutputAction()
            => $"SPELL WIND {(int)_targetPosition.X} {(int)_targetPosition.Y}";
    }
}
