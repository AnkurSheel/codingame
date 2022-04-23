using System.Numerics;

namespace SpringChallenge2022.Actions
{
    public class ControlSpellAction : IAction
    {
        private const int Range = 2200;
        private readonly int _id;
        private readonly Vector2 _targetPosition;

        public ControlSpellAction(int id, Vector2 targetPosition)
        {
            _id = id;
            _targetPosition = targetPosition;
        }

        public string GetOutputAction()
            => $"SPELL CONTROL {_id} {_targetPosition.X} {_targetPosition.Y}";
    }
}
