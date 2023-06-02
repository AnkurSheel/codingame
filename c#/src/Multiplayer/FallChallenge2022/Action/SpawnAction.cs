using FallChallenge2022.Models;
using SpringChallenge2021.Actions;

namespace FallChallenge2022.Action
{
    public class SpawnAction : IAction
    {
        private readonly Position _position;

        public SpawnAction(Position position)
        {
            _position = position;
        }

        public string GetOutputAction()
            => $"SPAWN 1 {_position.X} {_position.Y}";
    }
}
