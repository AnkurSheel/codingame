using System.Text;

using JoinThePac.Models;
using JoinThePac.Services;

namespace JoinThePac.Agents
{
    public class ReactAgent
    {
        private bool _alreadyWentToCenter;

        private readonly int _centerX;

        private readonly int _centerY;

        private readonly Game _game;

        public ReactAgent(Game game)
        {
            _game = game;
            _alreadyWentToCenter = false;

            _centerX = _game.Map.Width / 2;
            _centerY = _game.Map.Height / 2;

            var mapCell = _game.Map.Cells[_centerY, _centerX];
            while (mapCell.Type != CellType.Floor)
            {
                mapCell = _game.Map.Cells[_centerY + 1, _centerX + 1];
                _centerX = mapCell.X;
                _centerY = mapCell.Y;
            }
        }

        public string Think()
        {
            var action = new StringBuilder();

            foreach (var (_, pac) in _game.MyPlayer.Pacs)
            {
                if (pac.IsAlive)
                {
                    action.Append(GetMoveAction(pac));
                    action.Append(" | ");
                }
            }

            return action.ToString();
        }

        private string GetMoveAction(Pac pac)
        {
            var cell = _game.Map.Cells[pac.Y, pac.X];

            Io.Debug($"ID: {pac.Id} {cell.Neighbours.Count}");
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                Io.Debug($"neighbour {neighbour.X} {neighbour.Y} {neighbour.HasPellet}");
                if (neighbour.HasPellet && !IsPacInCell(neighbour))
                {
                    return $"MOVE {pac.Id} {neighbour.X} {neighbour.Y}";
                }
            }

            if (_alreadyWentToCenter || pac.X == _centerX && pac.Y == _centerY)
            {
                _alreadyWentToCenter = true;

                foreach (var mapCell in _game.Map.Cells)
                {
                    if (mapCell.HasPellet && !IsPacInCell(mapCell))
                    {
                        return $"MOVE {pac.Id} {mapCell.X} {mapCell.Y}";
                    }
                }
            }

            return pac.IsInSamePosition()
                       ? $"MOVE {pac.Id} {Constants.Random.Next(_game.Map.Width)} {Constants.Random.Next(_game.Map.Height)}"
                       : $"MOVE {pac.Id} {_centerX} {_centerY}";
        }

        private bool IsPacInCell(Cell mapCell)
        {
            foreach (var (_, pac) in _game.OpponentPlayer.Pacs)
            {
                if (pac.X == mapCell.X && pac.Y == _centerY)
                {
                    Io.Debug($"opponent {pac.Id}in {mapCell.X} {mapCell.Y}");
                    return true;
                }
            }

            foreach (var (_, pac) in _game.MyPlayer.Pacs)
            {
                if (pac.X == mapCell.X && pac.Y == _centerY)
                {
                    Io.Debug($"my pac {pac.Id}in {mapCell.X} {mapCell.Y}");
                    return true;
                }
            }

            return false;
        }
    }
}
