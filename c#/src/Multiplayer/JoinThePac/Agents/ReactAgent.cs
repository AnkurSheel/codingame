using System;
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
        private readonly Game _game;

        private readonly Dictionary<int, Cell> _chosenCells = new Dictionary<int, Cell>();

        private readonly HashSet<Cell> _moveCells = new HashSet<Cell>();

        private readonly Dictionary<int, IAction> _actions = new Dictionary<int, IAction>();

        public ReactAgent(Game game)
        {
            _game = game;
        }

        public string Think()
        {
            foreach (var chosenCell in _chosenCells)
            {
                Io.Debug($"Pac Id {chosenCell.Key} : Initial Chosen Cell {chosenCell.Value.Position}");
            }

            _actions.Clear();
            _moveCells.Clear();

            AddAbilityActions();

            AddMoveActions();

            return BuildOutput();
        }

        private void AddAbilityActions()
        {
            AddSpeedAction();
        }

        private void AddSpeedAction()
        {
            foreach (var (_, pac) in _game.MyPlayer.Pacs)
            {
                if (!_actions.ContainsKey(pac.Id) && pac.IsAlive && pac.AbilityCooldown == 0)
                {
                    _actions[pac.Id] = new SpeedAction();
                }
            }
        }

        private void AddSuperPelletActions()
        {
            var pathsToSuperPellets = GetAllPathBetweenPacsAndSuperPellets();

            foreach (var path in pathsToSuperPellets)
            {
                if (_actions.ContainsKey(path.Pac.Id))
                {
                    continue;
                }

                if (IsCellChosenBySomeoneElse(path.Pac.Id, path.Cell))
                {
                    continue;
                }

                if (path.Path.Count == 1)
                {
                    var cell = GetNextCellIfSpeed(path.Pac, path.Cell);
                    _actions[path.Pac.Id] = new MoveAction(cell.Position);
                    Io.Debug($"Pac Id {path.Pac.Id} : Super Pellet Position {path.Cell.Position} : Pac position {path.Pac.Position} : Path Count {path.Path.Count}");
                }
                else
                {
                    var cell = path.Path.First();
                    if (ShouldAvoidCell(path.Pac, cell))
                    {
                        continue;
                    }

                    if (path.Pac.SpeedTurnsLeft > 0)
                    {
                        var nextCell = path.Path.Skip(1).First();
                        if (ShouldAvoidCell(path.Pac, cell))
                        {
                            continue;
                        }

                        _moveCells.Add(nextCell);
                        _actions[path.Pac.Id] = new MoveAction(nextCell.Position);
                    }
                    else
                    {
                        _actions[path.Pac.Id] = new MoveAction(cell.Position);
                    }

                    _moveCells.Add(cell);

                    Io.Debug($"Pac Id {path.Pac.Id} : Super Pellet Position {path.Cell.Position} : Pac position {path.Pac.Position} : Path Count {path.Path.Count}");
                }

                _chosenCells[path.Pac.Id] = path.Cell;
            }
        }

        private void AddMoveActions()
        {
            AddSuperPelletActions();

            foreach (var (_, pac) in _game.MyPlayer.Pacs)
            {
                RemoveInvalidChosenCells(pac);

                if (!_actions.ContainsKey(pac.Id))
                {
                    if (pac.IsAlive)
                    {
                        _actions[pac.Id] = GetMoveAction(pac);
                    }
                }
            }
        }

        private MoveAction GetMoveAction(Pac pac)
        {
            var cell = _game.Map.Cells[pac.Position.Y, pac.Position.X];

            var action = GetMoveActionInSamePosition(pac, cell);
            if (action != null)
            {
                return action;
            }

            action = MoveToNeighbourPellet(pac, cell);
            if (action != null)
            {
                return action;
            }

            action = AddChosenCellAction(pac, cell);
            if (action != null)
            {
                return action;
            }

            action = MoveToRandomPellet(pac);
            if (action != null)
            {
                return action;
            }

            return new MoveAction(cell.Position);
        }

        private MoveAction GetMoveActionInSamePosition(Pac pac, Cell cell)
        {
            if (pac.IsInSamePosition() && pac.AbilityCooldown != 9)
            {
                Io.Debug($"Pac Id {pac.Id} : Same position {pac.Position}");
                var action = GetMoveIfInSamePosition(pac, cell);
                if (action != null)
                {
                    return action;
                }
            }

            return null;
        }

        private MoveAction AddChosenCellAction(Pac pac, Cell cell)
        {
            if (_chosenCells.ContainsKey(pac.Id) && _chosenCells[pac.Id].HasPellet)
            {
                var path = BFS.GetPath(cell, _chosenCells[pac.Id], GetObstacleCondition(pac));
                if (path != null)
                {
                    var nextCell = path.First();
                    _moveCells.Add(nextCell);

                    if (pac.SpeedTurnsLeft > 0 && path.Count >= 2)
                    {
                        nextCell = path.Skip(1).First();
                        _moveCells.Add(nextCell);
                    }

                    Io.Debug($"Pac Id: {pac.Id} : Chosen Cell Position {_chosenCells[pac.Id].Position} : Next cell position {nextCell.Position}");
                    {
                        return new MoveAction(nextCell.Position);
                    }
                }
            }

            return null;
        }

        private MoveAction GetMoveIfInSamePosition(Pac pac, Cell cell)
        {
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                if (!ShouldAvoidCell(pac, neighbour))
                {
                    Io.Debug($"Pac Id {pac.Id} :  Same Position Move {neighbour.Position}");
                    _chosenCells[pac.Id] = neighbour;
                    _moveCells.Add(neighbour);
                    return new MoveAction(neighbour.Position);
                }
            }

            return null;
        }

        private MoveAction MoveToNeighbourPellet(Pac pac, Cell cell)
        {
            var cellsToConsider = new List<Cell>();
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                if (neighbour.HasPellet && !ShouldAvoidCell(pac, neighbour))
                {
                    cellsToConsider.Add(neighbour);
                }
            }

            if (cellsToConsider.Any())
            {
                if (pac.SpeedTurnsLeft > 0)
                {
                    foreach (var cellConsider in cellsToConsider)
                    {
                        foreach (var (_, neighbour) in cellConsider.Neighbours)
                        {
                            if (neighbour.HasPellet && !ShouldAvoidCell(pac, neighbour))
                            {
                                Io.Debug($"Pac Id {pac.Id} : Neighbour Pellet Speed : {cellConsider.Position} : {neighbour.Position}");
                                _chosenCells[pac.Id] = neighbour;
                                _moveCells.Add(cellConsider);
                                _moveCells.Add(neighbour);
                                return new MoveAction(neighbour.Position);
                            }
                        }
                    }
                }

                var nextCell = cellsToConsider.First();
                Io.Debug($"Pac Id {pac.Id} : Neighbour Pellet : {nextCell.Position}");
                _chosenCells[pac.Id] = nextCell;
                _moveCells.Add(nextCell);
                return new MoveAction(nextCell.Position);
            }

            return null;
        }

        private MoveAction MoveToRandomPellet(Pac pac)
        {
            var cell = _game.Map.Cells[pac.Position.Y, pac.Position.X];
            var closestCells = BFS.GetClosestCells(cell, GetClosestCellCondition(pac), GetObstacleCondition(pac), 20);
            if (closestCells.Any())
            {
                var closestCell = closestCells.OrderByDescending(c => c.VisibleCells.Count()).First();
                Io.Debug($"Pac Id {pac.Id} : Random Uneaten pellet Position: {closestCell.Position}");
                _chosenCells[pac.Id] = closestCell;
                _moveCells.Add(closestCell);
                return new MoveAction(closestCell.Position);
            }

            return null;
        }

        private Cell GetNextCellIfSpeed(Pac pac, Cell cell)
        {
            _moveCells.Add(cell);

            if (pac.SpeedTurnsLeft > 0)
            {
                foreach (var (_, neighbour) in cell.Neighbours)
                {
                    if (!ShouldAvoidCell(pac, neighbour))
                    {
                        cell = neighbour;
                        if (!cell.HasPellet && neighbour.HasPellet)
                        {
                            break;
                        }
                    }
                }

                _moveCells.Add(cell);
            }

            return cell;
        }

        private List<PacPath> GetAllPathBetweenPacsAndSuperPellets()
        {
            var paths = new List<PacPath>();

            foreach (var (_, pac) in _game.MyPlayer.Pacs)
            {
                foreach (var superPellet in _game.Map.SuperPellets)
                {
                    if (!pac.IsAlive || _actions.ContainsKey(pac.Id))
                    {
                        continue;
                    }

                    var pacCell = _game.Map.Cells[pac.Position.Y, pac.Position.X];
                    var path = BFS.GetPath(pacCell, superPellet, GetObstacleCondition(pac));
                    if (path != null)
                    {
                        paths.Add(new PacPath(pac, superPellet, path));
                    }
                }
            }

            return paths.OrderBy(a => a.Path.Count).ToList();
        }

        private void RemoveInvalidChosenCells(Pac pac)
        {
            if (_chosenCells.ContainsKey(pac.Id))
            {
                var chosenCell = _chosenCells[pac.Id];

                if (!pac.IsAlive || pac.Position.IsSame(chosenCell.Position) || chosenCell.PelletValue == 0)
                {
                    Io.Debug($"removing Pac Id {pac.Id} : Chosen Cell {chosenCell.Position}");
                    _chosenCells.Remove(pac.Id);
                }
            }
        }

        private bool ShouldAvoidCell(Pac pac, Cell cell)
        {
            return _chosenCells.ContainsValue(cell)
                   || IsOpponentInCell(cell, opponentPac => pac.CanBeEaten(opponentPac.Type))
                   || GetPacInCell(cell, _game.MyPlayer.Pacs) != null
                   || _moveCells.Contains(cell);
        }

        private Func<Cell, bool> GetClosestCellCondition(Pac pac)
        {
            return currentCell => currentCell.Type == CellType.Floor && currentCell.HasPellet && !ShouldAvoidCell(pac, currentCell);
        }

        private Func<Cell, bool> GetObstacleCondition(Pac pac)
        {
            return currentCell => GetPacInCell(currentCell, _game.MyPlayer.Pacs) != null
                                  || IsOpponentInCell(currentCell, opponentPac => pac.CanBeEaten(opponentPac.Type))
                                  || _moveCells.Contains(currentCell);
        }

        private Pac GetPacInCell(Cell mapCell, Dictionary<int, Pac> pacs)
        {
            foreach (var (_, pac) in pacs)
            {
                if (pac.IsAlive && pac.Position.IsSame(mapCell.Position))
                {
                    return pac;
                }
            }

            return null;
        }

        private bool IsOpponentInCell(Cell currentCell, Func<Pac, bool> opponentPacCondition)
        {
            var open = new List<Cell> { currentCell };
            var seen = new HashSet<Cell> { currentCell };

            var count = 0;
            var maxCount = 2;
            while (open.Any())
            {
                var tempCell = open.First();
                open.RemoveAt(0);

                var opponentPac = GetPacInCell(tempCell, _game.OpponentPlayer.Pacs);
                if (opponentPac != null)
                {
                    if (opponentPacCondition(opponentPac))
                    {
                        return true;
                    }
                }

                if (count < maxCount)
                {
                    foreach (var (_, neighbour) in currentCell.Neighbours)
                    {
                        if (seen.Add(neighbour))
                        {
                            open.Add(neighbour);
                        }
                    }
                }

                count++;
            }

            return false;
        }

        private bool IsCellChosenBySomeoneElse(int pacId, Cell cell)
        {
            foreach (var (id, chosenCell) in _chosenCells)
            {
                if (cell.Equals(chosenCell) && id != pacId)
                {
                    return true;
                }
            }

            return false;
        }

        private string BuildOutput()
        {
            var output = new StringBuilder();
            foreach (var (pacId, action )in _actions)
            {
                output.Append(action.GetAction(pacId));
                output.Append(" | ");
            }

            return output.ToString();
        }
    }
}
