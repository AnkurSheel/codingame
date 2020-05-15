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
            Io.Debug("Chosen cells ");
            foreach (var chosenCell in _chosenCells)
            {
                Io.Debug($"{chosenCell.Key} : {chosenCell.Value.Position.X}, {chosenCell.Value.Position.Y}");
            }

            _actions.Clear();
            _moveCells.Clear();

            AddSpeedAction();

            AddSuperPelletActions();

            AddMoveActions();

            return BuildOutput();
        }

        private void AddSpeedAction()
        {
            foreach (var (_, pac) in _game.MyPlayer.Pacs)
            {
                if (pac.AbilityCooldown == 0)
                {
                    _actions[pac.Id] = new SpeedAction();
                }
            }
        }

        private void AddSuperPelletActions()
        {
            foreach (var superPellet in _game.Map.SuperPellets)
            {
                List<Cell> bestPath = null;
                Pac bestPac = null;

                foreach (var (_, pac )in _game.MyPlayer.Pacs)
                {
                    if (!pac.IsAlive || _actions.ContainsKey(pac.Id))
                    {
                        continue;
                    }

                    var pacCell = _game.Map.Cells[pac.Position.Y, pac.Position.X];
                    var path = BFS.GetPath(pacCell, superPellet, GetObstacleCondition(pac, pacCell));
                    if (path != null)
                    {
                        if (bestPath == null || path.Count < bestPath.Count)
                        {
                            bestPath = path;
                            bestPac = pac;
                        }
                    }
                }

                if (bestPac != null)
                {
                    _chosenCells[bestPac.Id] = superPellet;
                    if (bestPath.Count == 1)
                    {
                        _moveCells.Add(superPellet);
                        _actions[bestPac.Id] = new MoveAction(superPellet.Position);
                    }
                    else
                    {
                        var cell = bestPath.First();
                        _moveCells.Add(cell);
                        if (bestPac.SpeedTurnsLeft > 0 && bestPath.Count >= 2)
                        {
                            cell = bestPath.Skip(1).First();
                            _moveCells.Add(cell);
                        }

                        _actions[bestPac.Id] = new MoveAction(cell.Position);

                        Io.Debug($"Super Pellet : {superPellet.Position} : {bestPac.Id} : {bestPac.Position}");
                    }
                }
            }
        }

        private void AddMoveActions()
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

                if (!_actions.ContainsKey(pac.Id))
                {
                    if (pac.IsAlive)
                    {
                        _actions[pac.Id] = GetMoveAction(pac);
                    }
                    else
                    {
                        _chosenCells.Remove(pac.Id);
                    }
                }
            }
        }

        private MoveAction GetMoveAction(Pac pac)
        {
            var cell = _game.Map.Cells[pac.Position.Y, pac.Position.X];
            MoveAction action = null;
            if (pac.IsInSamePosition() && pac.SpeedTurnsLeft != 5)
            {
                Io.Debug($"{pac.Id} Same position {pac.Position}");
                action = GetMoveIfInSamePosition(pac, cell);
                if (action != null)
                {
                    return action;
                }
            }

            action = MoveToNeighbourPellet(pac, cell);
            if (action != null)
            {
                return action;
            }

            if (_chosenCells.ContainsKey(pac.Id))
            {
                var path = BFS.GetPath(cell, _chosenCells[pac.Id], GetObstacleCondition(pac, cell));
                if (path != null)
                {
                    var nextCell = path.First();
                    _moveCells.Add(nextCell);

                    if (pac.SpeedTurnsLeft > 0 && path.Count >= 2)
                    {
                        nextCell = path.Skip(1).First();
                        _moveCells.Add(nextCell);
                    }

                    Io.Debug($"{pac.Id} Chosen Cell {_chosenCells[pac.Id].Position} {nextCell.Position}");
                    return new MoveAction(nextCell.Position);
                }
            }

            action = MoveToRandomPellet(pac);
            if (action != null)
            {
                return action;
            }

            return new MoveAction(cell.Position);
            ;
        }

        private MoveAction GetMoveIfInSamePosition(Pac pac, Cell cell)
        {
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                if (!_chosenCells.ContainsValue(neighbour)
                    && !IsOpponentInCell(pac, neighbour)
                    && GetPacInCell(neighbour, _game.MyPlayer.Pacs) == null
                    && !_moveCells.Contains(neighbour))
                {
                    Io.Debug($"{pac.Id} Neighbour {neighbour.Position}");
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
                if (neighbour.HasPellet
                    && !_chosenCells.ContainsValue(neighbour)
                    && !IsOpponentInCell(pac, neighbour)
                    && GetPacInCell(neighbour, _game.MyPlayer.Pacs) == null
                    && !_moveCells.Contains(neighbour))
                {
                    cellsToConsider.Add(neighbour);
                }
            }

            if (cellsToConsider.Any())
            {
                foreach (var cellConsider in cellsToConsider)
                {
                    foreach (var (_, neighbour) in cellConsider.Neighbours)
                    {
                        if (neighbour.HasPellet
                            && !_chosenCells.ContainsValue(neighbour)
                            && !IsOpponentInCell(pac, neighbour)
                            && GetPacInCell(neighbour, _game.MyPlayer.Pacs) == null
                            && !_moveCells.Contains(neighbour))
                        {
                            if (pac.SpeedTurnsLeft > 0)
                            {
                                Io.Debug($"{pac.Id} Neighbour Pellet : {cellConsider.Position} :{neighbour.Position}");
                                _chosenCells[pac.Id] = neighbour;
                                _moveCells.Add(cellConsider);
                                _moveCells.Add(neighbour);
                                return new MoveAction(neighbour.Position);
                            }
                            else
                            {
                                var nextCell = cellsToConsider.First();
                                Io.Debug($"{pac.Id} Neighbour Pellet : {nextCell.Position}");
                                _chosenCells[pac.Id] = nextCell;
                                _moveCells.Add(neighbour);
                                return new MoveAction(nextCell.Position);
                            }
                        }
                    }
                }
            }

            return null;
        }

        private MoveAction MoveToRandomPellet(Pac pac)
        {
            var closestCell = GetClosestCell(pac);
            if (closestCell != null)
            {
                Io.Debug($"{pac.Id} Random Uneaten pellet {closestCell.Position}");
                _chosenCells[pac.Id] = closestCell;
                _moveCells.Add(closestCell);
                return new MoveAction(closestCell.Position);
            }

            return null;
        }

        private Cell GetClosestCell(Pac pac)
        {
            var cell = _game.Map.Cells[pac.Position.Y, pac.Position.X];
            return BFS.GetClosestCell(cell, GetClosestCellCondition(pac), GetObstacleCondition(pac, cell));
        }

        private Func<Cell, bool> GetClosestCellCondition(Pac pac)
        {
            return currentCell => currentCell.Type == CellType.Floor
                                  && currentCell.HasPellet
                                  && !_chosenCells.ContainsValue(currentCell)
                                  && GetPacInCell(currentCell, _game.MyPlayer.Pacs) == null
                                  && !IsOpponentInCell(pac, currentCell)
                                  && !_moveCells.Contains(currentCell);
        }

        private Func<Cell, bool> GetObstacleCondition(Pac pac, Cell cell)
        {
            return currentCell => GetPacInCell(currentCell, _game.MyPlayer.Pacs) != null
                                  || IsOpponentInCell(pac, currentCell)
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

        private bool IsOpponentInCell(Pac pac, Cell currentCell)
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
                    if (!pac.CanEat(opponentPac.Type))
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
