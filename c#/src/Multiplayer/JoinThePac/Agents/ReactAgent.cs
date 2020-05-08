using System.Linq;
using System.Text;

using JoinThePac.Models;

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

            foreach (var pac in _game.MyPlayer.Pacs)
            {
                var cell = _game.Map.Cells[pac.Y, pac.X];
                action.Append(GetMoveActionForPac(cell, pac));
                action.Append(" | ");
            }

            return action.ToString();
        }

        private string GetMoveActionForPac(Cell cell, Pac pac)
        {
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                if (neighbour.HasPellet && !IsOpponentPacInCell(neighbour))
                {
                    {
                        return $"MOVE {pac.Id} {neighbour.X} {neighbour.Y}";
                    }
                }
            }

            if (_alreadyWentToCenter || pac.X == _centerX && pac.Y == _centerY)
            {
                _alreadyWentToCenter = true;

                foreach (var mapCell in _game.Map.Cells)
                {
                    if (mapCell.HasPellet && !IsOpponentPacInCell(mapCell))
                    {
                        {
                            return $"MOVE {pac.Id} {mapCell.X} {mapCell.Y}";
                        }
                    }
                }
            }

            return $"MOVE {pac.Id} {_centerX} {_centerY}";
        }

        private bool IsOpponentPacInCell(Cell mapCell)
        {
            return _game.OpponentPlayer.Pacs.Any(pac => pac.X != mapCell.X && pac.Y == _centerY);
        }
    }
}
