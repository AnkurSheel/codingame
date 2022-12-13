using SpringChallenge2021.Actions;

namespace FallChallenge2022.Action
{
    public class MoveAction : IAction
    {
        private readonly int _fromPosX;
        private readonly int _fromPosY;
        private readonly int _toPosX;
        private readonly int _toPosY;

        public MoveAction(
            int fromPosX,
            int fromPosY,
            int toPosX,
            int toPosY)
        {
            _fromPosX = fromPosX;
            _fromPosY = fromPosY;
            _toPosX = toPosX;
            _toPosY = toPosY;
        }

        public string GetOutputAction()
            => $"MOVE 1 {_fromPosX} {_fromPosY} {_toPosX} {_toPosY}";
    }
}
