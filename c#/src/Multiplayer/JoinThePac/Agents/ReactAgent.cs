using JoinThePac.Models;
using JoinThePac.Services;

namespace JoinThePac.Agents
{
    public class ReactAgent
    {
        private readonly Map _map;

        private readonly Player _myPlayer;

        private bool _alreadyWentToCenter;

        private readonly int _centerX;

        private readonly int _centerY;

        public ReactAgent(Map map, Player myPlayer)
        {
            _map = map;
            _myPlayer = myPlayer;
            _alreadyWentToCenter = false;

            _centerX = _map.Width / 2;
            _centerY = _map.Height / 2;

            var mapCell = map.Cells[_centerY, _centerX];
            while (mapCell.Type != CellType.Floor)
            {
                mapCell = map.Cells[_centerY + 1, _centerX + 1];
                _centerX = mapCell.X;
                _centerY = mapCell.Y;
            }
        }

        public string GetAction()
        {
            var pac = _myPlayer.Pac;
            var cell = _map.Cells[pac.Y, pac.X];
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

                foreach (var mapCell in _map.Cells)
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
