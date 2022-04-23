using SpringChallenge2022.Agents;

namespace SpringChallenge2022.Actions
{
    internal class MoveAction : IAction
    {
        private readonly Vector _position;

        public MoveAction(Vector position)
        {
            _position = position;
        }

        public string GetOutputAction()
            => $"MOVE {_position.X} {_position.Y}";
    }
}
