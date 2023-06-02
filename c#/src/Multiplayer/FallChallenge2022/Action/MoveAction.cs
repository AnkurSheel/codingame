using FallChallenge2022.Models;
using SpringChallenge2021.Actions;

namespace FallChallenge2022.Action
{
    public class MoveAction : IAction
    {
        private readonly Position _from;
        private readonly Position _to;

        public MoveAction(
            Position from,
            Position to)
        {
            _from = from;
            _to = to;
        }

        public string GetOutputAction()
            => $"MOVE 1 {_from.X} {_from.Y} {_to.X} {_to.Y}";
    }
}
