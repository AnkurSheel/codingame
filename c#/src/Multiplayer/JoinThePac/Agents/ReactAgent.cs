using System.Collections.Generic;
using System.Linq;
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

            Io.Debug("Chosen cells ");
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
                        Io.Debug($"removing {pac.Id} {chosenCell.Position.X} {chosenCell.Position.Y}");
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
                var path = BFS.GetPath(cell, _chosenCells[pac.Id], currentCell => IsPacInCell(currentCell));
                if (path != null)
                {
                    var nextCell = path.First();
                    Io.Debug($"{pac.Id} Chosen Cell {_chosenCells[pac.Id].Position} {nextCell.Position}");
                    return $"MOVE {pac.Id} {nextCell.Position.X} {nextCell.Position.Y}";
                }
            }

            var superPellet = GetSuperPellet(cell);
            if (superPellet != null)
            {
                var path = BFS.GetPath(cell, superPellet, currentCell => IsPacInCell(currentCell));
                if (path != null)
                {
                    var nextCell = path.First();
                    _chosenCells[pac.Id] = superPellet;

                    Io.Debug($"{pac.Id} Super Pellet {superPellet.Position} {nextCell.Position}");
                    return $"MOVE {pac.Id} {nextCell.Position.X} {nextCell.Position.Y}";
                }
            }

            var action = MoveToNeighbour(pac, cell);
            if (!string.IsNullOrEmpty(action))
            {
                return action;
            }

            if (_alreadyWentToCenter || pac.Position.IsSame(_centerPosition))
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
            var cell = _game.Map.Cells[pac.Position.Y, pac.Position.X];
            var closestCell = BFS.GetClosestCell(cell,
                                                 currentCell => currentCell.Type == CellType.Floor
                                                                && (currentCell.HasPellet || currentCell.PelletValue == -1)
                                                                && !_chosenCells.ContainsValue(currentCell)
                                                                && !IsPacInCell(currentCell));
            if (closestCell != null)
            {
                Io.Debug($"{pac.Id} Random pellet {closestCell.Position}");
                _chosenCells[pac.Id] = closestCell;
                return $"MOVE {pac.Id} {closestCell.Position.X} {closestCell.Position.Y}";
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
                    _chosenCells[pac.Id] = neighbour;
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
            return BFS.GetClosestCell(cell, currentCell => currentCell.HasSuperPellet && !_chosenCells.ContainsValue(currentCell));
        }

        private bool IsPacInCell(Cell mapCell)
        {
            foreach (var (_, pac) in _game.OpponentPlayer.Pacs)
            {
                if (pac.Position.IsSame(mapCell.Position))
                {
                    return true;
                }
            }

            foreach (var (_, pac) in _game.MyPlayer.Pacs)
            {
                if (pac.Position.IsSame(mapCell.Position))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
