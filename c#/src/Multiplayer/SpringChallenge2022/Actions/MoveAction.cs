using System.Numerics;

namespace SpringChallenge2022.Actions
{
    internal class MoveAction : IAction
    {
        private readonly Vector2 _position;

        public MoveAction(Vector2 position)
        {
            _position = position;
        }

        public string GetOutputAction()
            => $"MOVE {(int)_position.X} {(int)_position.Y}";
    }
}
