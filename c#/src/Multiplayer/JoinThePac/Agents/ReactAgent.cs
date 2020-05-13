using System.Collections.Generic;
using System.Linq;
using System.Text;

using JoinThePac.Actions;
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

        private readonly Dictionary<int, MoveAction> Actions = new Dictionary<int, MoveAction>();

        private int _pacCount;

        public ReactAgent(Game game)
        {
            _game = game;
            _alreadyWentToCenter = false;
            _pacCount = _game.MyPlayer.Pacs.Count;
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
            Io.Debug("Chosen cells ");
            foreach (var chosenCell in _chosenCells)
            {
                Io.Debug($"{chosenCell.Key} : {chosenCell.Value.Position.X}, {chosenCell.Value.Position.Y}");
            }

            Actions.Clear();

            AddSuperPelletActions();

            AddActionsForPacs();

            return BuildOutput();
        }

        private void AddActionsForPacs()
        {
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

                if (pac.IsAlive && !Actions.ContainsKey(pac.Id))
                {
                    Actions[pac.Id] = GetMoveAction(pac);
                }
                else
                {
                    _chosenCells.Remove(pac.Id);
                }
            }
        }

        private string BuildOutput()
        {
            var output = new StringBuilder();
            foreach (var (pacId, action )in Actions)
            {
                output.Append(action.GetAction(pacId));
                output.Append(" | ");
            }

            return output.ToString();
        }

        private void AddSuperPelletActions()
        {
            foreach (var superPellet in _game.Map.SuperPellets)
            {
                List<Cell> bestPath = null;
                Pac bestPac = null;

                foreach (var (_, pac )in _game.MyPlayer.Pacs)
                {
                    if (!pac.IsAlive || Actions.ContainsKey(pac.Id))
                    {
                        continue;
                    }

                    var pacCell = _game.Map.Cells[pac.Position.Y, pac.Position.X];
                    var path = BFS.GetPath(superPellet,
                                           pacCell,
                                           currentCell => !currentCell.Equals(pacCell)
                                                          && (IsPacInCell(currentCell, _game.MyPlayer.Pacs)
                                                              || IsPacInCell(currentCell, _game.OpponentPlayer.Pacs)));
                    if (path != null)
                    {
                        Io.Debug($"Super Pellet : {pac.Id} : {superPellet.Position} : {pac.Position}: {path.Count}");

                        if (bestPath == null || path.Count < bestPath.Count)
                        {
                            bestPath = path;
                            bestPac = pac;
                            Io.Debug($"Best Path : {pac.Id} : {superPellet.Position} : {pac.Position}: {path.Count}");
                        }
                    }
                }

                if (bestPac != null)
                {
                    _chosenCells[bestPac.Id] = superPellet;
                    if (bestPath.Count == 1)
                    {
                        Actions[bestPac.Id] = new MoveAction(superPellet.Position);
                    }
                    else
                    {
                        Actions[bestPac.Id] = new MoveAction(bestPath.First().Position);
                    }
                }
            }
        }

        private MoveAction GetMoveAction(Pac pac)
        {
            var cell = _game.Map.Cells[pac.Position.Y, pac.Position.X];
            if (pac.IsInSamePosition())
            {
                Io.Debug($"{pac.Id} Same position {pac.Position}");
                return GetMoveIfInSamePosition(pac, cell);
            }

            var action = MoveToNeighbourPellet(pac, cell);
            if (action != null)
            {
                return action;
            }

            if (_chosenCells.ContainsKey(pac.Id))
            {
                var path = BFS.GetPath(cell,
                                       _chosenCells[pac.Id],
                                       currentCell => IsPacInCell(currentCell, _game.OpponentPlayer.Pacs)
                                                      || IsPacInCell(currentCell, _game.MyPlayer.Pacs));
                if (path != null)
                {
                    var nextCell = path.First();
                    Io.Debug($"{pac.Id} Chosen Cell {_chosenCells[pac.Id].Position} {nextCell.Position}");
                    return new MoveAction(nextCell.Position);
                }
            }

            if (_alreadyWentToCenter || pac.Position.IsSame(_centerPosition))
            {
                var moveAction = MoveToRandomPellet(pac);
                if (moveAction != null)
                {
                    return moveAction;
                }
            }

            Io.Debug($"{pac.Id} Center {_centerPosition}");
            return new MoveAction(_centerPosition);
        }

        private MoveAction MoveToRandomPellet(Pac pac)
        {
            _alreadyWentToCenter = true;
            var cell = _game.Map.Cells[pac.Position.Y, pac.Position.X];
            var closestCell = BFS.GetClosestCell(cell,
                                                 currentCell => currentCell.Type == CellType.Floor
                                                                && currentCell.HasPellet
                                                                && !_chosenCells.ContainsValue(currentCell)
                                                                && !IsPacInCell(currentCell, _game.OpponentPlayer.Pacs)
                                                                && !IsPacInCell(currentCell, _game.MyPlayer.Pacs));
            if (closestCell != null)
            {
                Io.Debug($"{pac.Id} Random Uneaten pellet {closestCell.Position}");
                _chosenCells[pac.Id] = closestCell;
                return new MoveAction(closestCell.Position);
            }
            else
            {
                closestCell = BFS.GetClosestCell(cell,
                                                 currentCell => currentCell.Type == CellType.Floor
                                                                && currentCell.PelletValue == -1
                                                                && !_chosenCells.ContainsValue(currentCell)
                                                                && !IsPacInCell(currentCell, _game.OpponentPlayer.Pacs)
                                                                && !IsPacInCell(currentCell, _game.MyPlayer.Pacs));

                if (closestCell != null)
                {
                    Io.Debug($"{pac.Id} Random Unknown pellet {closestCell.Position}");
                    _chosenCells[pac.Id] = closestCell;
                    return new MoveAction(closestCell.Position);
                }
            }

            return null;
        }

        private MoveAction MoveToNeighbourPellet(Pac pac, Cell cell)
        {
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                if (neighbour.HasPellet
                    && !_chosenCells.ContainsValue(neighbour)
                    && !IsPacInCell(neighbour, _game.OpponentPlayer.Pacs)
                    && !IsPacInCell(neighbour, _game.MyPlayer.Pacs))
                {
                    Io.Debug($"{pac.Id} Neighbour Pellet {neighbour.Position}");
                    _chosenCells[pac.Id] = neighbour;
                    return new MoveAction(neighbour.Position);
                }
            }

            return null;
        }

        private MoveAction GetMoveIfInSamePosition(Pac pac, Cell cell)
        {
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                if (!_chosenCells.ContainsValue(neighbour)
                    && !IsPacInCell(neighbour, _game.OpponentPlayer.Pacs)
                    && !IsPacInCell(neighbour, _game.MyPlayer.Pacs))
                {
                    Io.Debug($"{pac.Id} Neighbour {neighbour.Position}");
                    _chosenCells[pac.Id] = neighbour;
                    return new MoveAction(neighbour.Position);
                }
            }

            Io.Debug($"{pac.Id} Random");
            var randomPosition = new Coordinate(Constants.Random.Next(_game.Map.Width), Constants.Random.Next(_game.Map.Height));
            return new MoveAction(randomPosition);
        }

        private Cell GetSuperPellet(Cell cell)
        {
            return BFS.GetClosestCell(cell, currentCell => currentCell.HasSuperPellet && !_chosenCells.ContainsValue(currentCell));
        }

        private bool IsPacInCell(Cell mapCell, Dictionary<int, Pac> pacs)
        {
            foreach (var (_, pac) in pacs)
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
