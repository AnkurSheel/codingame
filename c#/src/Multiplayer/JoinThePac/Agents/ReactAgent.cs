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
            var pac = _game.MyPlayer.Pac;
            var cell = _game.Map.Cells[pac.Y, pac.X];
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                if (neighbour.HasPellet)
                {
                    return $"MOVE {pac.Id} {neighbour.X} {neighbour.Y}";
                }
            }

            if (_alreadyWentToCenter || pac.X == _centerX && pac.Y == _centerY)
            {
                Io.Debug("here");
                _alreadyWentToCenter = true;

                foreach (var mapCell in _game.Map.Cells)
                {
                    if (mapCell.HasPellet)
                    {
                        return $"MOVE {pac.Id} {mapCell.X} {mapCell.Y}";
                    }
                }
            }

            return $"MOVE {pac.Id} {_centerX} {_centerY}";
        }
    }
}
