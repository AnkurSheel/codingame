using JoinThePac.Models;

namespace JoinThePac.Actions
{
    internal class MoveAction
    {
        public MoveAction(Coordinate position)
        {
            Position = position;
        }

        public Coordinate Position { get; }

        public string GetAction(int id)
        {
            return $"MOVE {id} {Position.X} {Position.Y}";
        }
    }
}
