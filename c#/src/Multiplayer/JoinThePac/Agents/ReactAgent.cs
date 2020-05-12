using System.Collections.Generic;
using System.Text;

using JoinThePac.Models;
using JoinThePac.Services;

namespace JoinThePac.Agents
{
    public class ReactAgent
    {
        private bool _alreadyWentToCenter;

        private readonly Coordinate _centerPosition;

        private readonly Game _game;

        private readonly HashSet<Cell> _chosenCells = new HashSet<Cell>();

        public ReactAgent(Game game)
        {
            _game = game;
            _alreadyWentToCenter = false;

            _centerPosition = new Coordinate(_game.Map.Width / 2, _game.Map.Height / 2);

            var mapCell = _game.Map.Cells[_centerPosition.Y, _centerPosition.X];
            while (mapCell.Type != CellType.Floor)
            {
                mapCell = _game.Map.Cells[_centerPosition.Y + 1, _centerPosition.X + 1];
                _centerPosition.Update(mapCell.Position);
            }
        }

        public string Think()
        {
            _chosenCells.Clear();
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
            var cell = _game.Map.Cells[pac.Position.Y, pac.Position.X];
            if (pac.IsInSamePosition())
            {
                return GetMoveIfInSamePosition(pac, cell);
            }

            var superPellet = GetSuperPellet();
            if (superPellet != null)
            {
                _chosenCells.Add(superPellet);
                return $"MOVE {pac.Id} {superPellet.Position.X} {superPellet.Position.Y}";
            }

            var action = MoveToNeighbour(pac, cell);
            if (!string.IsNullOrEmpty(action))
            {
                return action;
            }

            if (_alreadyWentToCenter || pac.Position.Equals(_centerPosition))
            {
                var moveAction = MoveToRandomPellet(pac);
                if (!string.IsNullOrEmpty(moveAction))
                {
                    return moveAction;
                }
            }

            return $"MOVE {pac.Id} {_centerPosition.X} {_centerPosition.Y}";
        }

        private string MoveToRandomPellet(Pac pac)
        {
            _alreadyWentToCenter = true;

            foreach (var mapCell in _game.Map.Cells)
            {
                if ((mapCell.HasPellet || mapCell.PelletValue == -1) && !_chosenCells.Contains(mapCell) && !IsPacInCell(mapCell))
                {
                    _chosenCells.Add(mapCell);
                    return $"MOVE {pac.Id} {mapCell.Position.X} {mapCell.Position.Y}";
                }
            }

            return null;
        }

        private string MoveToNeighbour(Pac pac, Cell cell)
        {
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                Io.Debug($"neighbour {neighbour.Position.X} {neighbour.Position.Y} {neighbour.HasPellet}");
                if (neighbour.HasPellet && !_chosenCells.Contains(neighbour) && !IsPacInCell(neighbour))
                {
                    _chosenCells.Add(neighbour);
                    return $"MOVE {pac.Id} {neighbour.Position.X} {neighbour.Position.Y}";
                }
            }

            return null;
        }

        private string GetMoveIfInSamePosition(Pac pac, Cell cell)
        {
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                if (!_chosenCells.Contains(neighbour) && !IsPacInCell(neighbour))
                {
                    _chosenCells.Add(neighbour);
                    return $"MOVE {pac.Id} {neighbour.Position.X} {neighbour.Position.Y}";
                }
            }

            return $"MOVE {pac.Id} {Constants.Random.Next(_game.Map.Width)} {Constants.Random.Next(_game.Map.Height)}";
        }

        private Cell GetSuperPellet()
        {
            foreach (var cell in _game.Map.Cells)
            {
                if (cell.HasSuperPellet && !_chosenCells.Contains(cell))
                {
                    return cell;
                }
            }

            return null;
        }

        private bool IsPacInCell(Cell mapCell)
        {
            foreach (var (_, pac) in _game.OpponentPlayer.Pacs)
            {
                if (pac.Position.Equals(mapCell.Position))
                {
                    return true;
                }
            }

            foreach (var (_, pac) in _game.MyPlayer.Pacs)
            {
                if (pac.Position.Equals(mapCell.Position))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
