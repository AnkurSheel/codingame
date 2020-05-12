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

        private readonly Dictionary<int, Cell> _chosenCells = new Dictionary<int, Cell>();

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
            var action = new StringBuilder();

            Io.Debug($"Chosen cells ");
            foreach (var chosenCell in _chosenCells)
            {
                Io.Debug($"{chosenCell.Key} : {chosenCell.Value.Position.X}, {chosenCell.Value.Position.Y}");
            }
            foreach (var (_, pac) in _game.MyPlayer.Pacs)
            {
                if (_chosenCells.ContainsKey(pac.Id))
                {
                    var chosenCell = _chosenCells[pac.Id];
                    if (pac.Position.IsSame(chosenCell.Position) || chosenCell.PelletValue == 0)
                    {
                        Io.Debug($"removing {pac.Id} { chosenCell.Position.X} {chosenCell.Position.Y}");
                        _chosenCells.Remove(pac.Id);

                    }
                }
                if (pac.IsAlive)
                {
                    action.Append(GetMoveAction(pac));
                    action.Append(" | ");
                }
                else
                {
                    _chosenCells.Remove(pac.Id);
                }
            }

            return action.ToString();
        }

        private string GetMoveAction(Pac pac)
        {
            var cell = _game.Map.Cells[pac.Position.Y, pac.Position.X];
            if (pac.IsInSamePosition())
            {
                Io.Debug($"{pac.Id} Same position {pac.Position}");
                return GetMoveIfInSamePosition(pac, cell);
            }

            if (_chosenCells.ContainsKey(pac.Id))
            {
                var position = _chosenCells[pac.Id].Position;
                Io.Debug($"{pac.Id} Chosen Cell {position}");
                return $"MOVE {pac.Id} {position.X} {position.Y}";
            }

            var superPellet = GetSuperPellet(cell);
            if (superPellet != null)
            {
                _chosenCells.Add(pac.Id, superPellet);
                Io.Debug($"{pac.Id} Super Pellet {superPellet.Position}");
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

            Io.Debug($"{pac.Id} Center {_centerPosition}");
            return $"MOVE {pac.Id} {_centerPosition.X} {_centerPosition.Y}";
        }

        private string MoveToRandomPellet(Pac pac)
        {
            _alreadyWentToCenter = true;

            foreach (var mapCell in _game.Map.Cells)
            {
                if ((mapCell.HasPellet || mapCell.PelletValue == -1) && !_chosenCells.ContainsValue(mapCell) && !IsPacInCell(mapCell))
                {
                    Io.Debug($"{pac.Id} Random pellet {mapCell.Position}");
                    _chosenCells[pac.Id] = mapCell;
                    return $"MOVE {pac.Id} {mapCell.Position.X} {mapCell.Position.Y}";
                }
            }

            return null;
        }

        private string MoveToNeighbour(Pac pac, Cell cell)
        {
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                if (neighbour.HasPellet && !_chosenCells.ContainsValue(neighbour) && !IsPacInCell(neighbour))
                {
                    Io.Debug($"{pac.Id} Neighbour {neighbour.Position}");
                    _chosenCells.Add(pac.Id, neighbour);
                    return $"MOVE {pac.Id} {neighbour.Position.X} {neighbour.Position.Y}";
                }
            }

            return null;
        }

        private string GetMoveIfInSamePosition(Pac pac, Cell cell)
        {
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                if (!_chosenCells.ContainsValue(neighbour) && !IsPacInCell(neighbour))
                {
                    Io.Debug($"{pac.Id} Neighbour {neighbour.Position}");
                    _chosenCells[pac.Id] = neighbour;
                    return $"MOVE {pac.Id} {neighbour.Position.X} {neighbour.Position.Y}";
                }
            }

            Io.Debug($"{pac.Id} Random");
            return $"MOVE {pac.Id} {Constants.Random.Next(_game.Map.Width)} {Constants.Random.Next(_game.Map.Height)}";
        }

        private Cell GetSuperPellet(Cell cell)
        {
            return BFS.GetClosestSuperPelletCell(cell, _chosenCells);
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
