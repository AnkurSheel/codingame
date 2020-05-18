using System;
using System.Text;
using JoinThePac.Models;
using JoinThePac.Services;
using JoinThePac.Agents;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JoinThePac.Actions;
using System.IO;


// 16/05/2020 07:42


namespace JoinThePac
{
    public static class Constants
    {
        public static readonly bool IsDebugOn = true;

        public static readonly bool IsForInput = false;

        public static readonly bool IsLocalRun = false;

        public static readonly Random Random = new Random(123);
    }
}


namespace JoinThePac
{
    public class Game
    {
        public Game()
        {
            MyPlayer = new Player();
            OpponentPlayer = new Player();
        }

        public Map Map { get; private set; }

        public Player MyPlayer { get; }

        public Player OpponentPlayer { get; }

        public void InitializeMap()
        {
            var inputs = Io.ReadLine().Split(' ');
            var width = int.Parse(inputs[0]); // size of the grid
            var height = int.Parse(inputs[1]); // top left corner is (x=0, y=0)

            Map = new Map(width, height);

            for (var i = 0; i < height; i++)
            {
                var row = Io.ReadLine(); // one line of the grid: space " " is floor, pound "#" is wall
                Map.Build(i, row);
            }

            Map.SetupCells();
            //Io.Debug($"Built Map{Environment.NewLine}{map}");
        }

        public void ReadTurn()
        {
            Reset();
            var inputs = Io.ReadLine().Split(' ');
            MyPlayer.Score = int.Parse(inputs[0]);

            OpponentPlayer.Score = int.Parse(inputs[1]);
            var visiblePacCount = int.Parse(Io.ReadLine()); // all your pacs and enemy pacs in sight

            for (var i = 0; i < visiblePacCount; i++)
            {
                inputs = Io.ReadLine().Split(' ');
                var pacId = int.Parse(inputs[0]); // pac number (unique within a team)
                var mine = inputs[1] != "0"; // true if this pac is yours
                var x = int.Parse(inputs[2]); // position in the grid
                var y = int.Parse(inputs[3]); // position in the grid
                var typeId = inputs[4]; // unused in wood leagues
                var speedTurnsLeft = int.Parse(inputs[5]); // unused in wood leagues
                var abilityCooldown = int.Parse(inputs[6]); // unused in wood leagues
                Map.SetCellValue(x, y, 0);
                if (mine)
                {
                    MyPlayer.UpdatePac(pacId, x, y, PacTypeExtensions.FromString(typeId), speedTurnsLeft, abilityCooldown);
                }
                else
                {
                    OpponentPlayer.UpdatePac(pacId, x, y, PacTypeExtensions.FromString(typeId), speedTurnsLeft, abilityCooldown);
                }
            }

            Map.ResetSuperPellets();
            MyPlayer.ResetVisibleCells(Map);

            var visiblePelletCount = int.Parse(Io.ReadLine()); // all pellets in sight
            for (var i = 0; i < visiblePelletCount; i++)
            {
                inputs = Io.ReadLine().Split(' ');
                var x = int.Parse(inputs[0]);
                var y = int.Parse(inputs[1]);
                var value = int.Parse(inputs[2]); // amount of points this pellet is worth
                Map.SetCellValue(x, y, value);
            }

            //DebugPelletValues();
        }

        private void Reset()
        {
            MyPlayer.Reset();
            OpponentPlayer.Reset();
        }

        private void DebugPelletValues()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Map.Height; i++)
            {
                for (var j = 0; j < Map.Width; j++)
                {
                    if (Map.Cells[i, j].Type == CellType.Floor)
                    {
                        var pelletValue = Map.Cells[i, j].PelletValue;
                        var a = 'U';
                        if (pelletValue == 10)
                        {
                            a = 'S';
                        }
                        else if (pelletValue == 0)
                        {
                            a = 'E';
                        }
                        else if (pelletValue == 1)
                        {
                            a = 'P';
                        }

                        sb.Append($"{a}");
                    }
                    else if (Map.Cells[i, j].Type == CellType.Wall)
                    {
                        sb.Append("#");
                    }
                }

                sb.AppendLine();
            }

            Io.Debug(sb.ToString());
        }
    }
}

namespace JoinThePac
{
    /**
 * Grab the pellets as fast as you can!
 **/
    internal class JoinThePac
    {
        private static void Main(string[] args)
        {
            Io.Initialize();

            var game = new Game();

            game.InitializeMap();

            var agent = new ReactAgent(game);

            int i = 0;
            // game loop
            while (true)
            {
                game.ReadTurn();
                Io.Debug($"Turn {i}");
                i++;
                Io.WriteLine(agent.Think()); // MOVE <pacId> <x> <y>
            }
        }
    }
}
namespace JoinThePac.Actions
{
    internal interface IAction
    {
        string GetAction(int id);
    }
}

namespace JoinThePac.Actions
{
    internal class MoveAction : IAction
    {
        public MoveAction(Coordinate position)
        {
            Position = position;
        }

        public Coordinate Position { get; }

        public string GetAction(int id)
        {
            return $"MOVE {id} {Position.X} {Position.Y}";
        }
    }
}
namespace JoinThePac.Actions
{
    internal class SpeedAction : IAction
    {
        public string GetAction(int id)
        {
            return $"SPEED {id}";
        }
    }
}


namespace JoinThePac.Agents
{
    [DebuggerDisplay("pac={Pac.Id} cell={Cell.Position} path={Path.Count}")]
    public class PacPath
    {
        public Pac Pac { get; }

        public Cell Cell { get; }

        public List<Cell> Path { get; }

        public PacPath(Pac pac, Cell cell, List<Cell> path)
        {
            Pac = pac;
            Cell = cell;
            Path = path;
        }
    }
}


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
            if (pac.IsInSamePosition() && pac.SpeedTurnsLeft != 5)
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
            var closestCells = BFS.GetClosestCells(cell, GetClosestCellCondition(pac), GetObstacleCondition(pac, cell), 20);
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
                    var path = BFS.GetPath(pacCell, superPellet, GetObstacleCondition(pac, pacCell));
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

        private bool ShouldAvoidCell(Pac pac, Cell neighbour)
        {
            return _chosenCells.ContainsValue(neighbour)
                   || IsOpponentInCell(pac, neighbour)
                   || GetPacInCell(neighbour, _game.MyPlayer.Pacs) != null
                   || _moveCells.Contains(neighbour);
        }

        private Func<Cell, bool> GetClosestCellCondition(Pac pac)
        {
            return currentCell => currentCell.Type == CellType.Floor && currentCell.HasPellet && !ShouldAvoidCell(pac, currentCell);
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
                    if (pac.CanBeEaten(opponentPac.Type))
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
            foreach (var (pacId, action) in _actions)
            {
                output.Append(action.GetAction(pacId));
                output.Append(" | ");
            }

            return output.ToString();
        }
    }
}

namespace JoinThePac.Models
{
    [DebuggerDisplay("Position = {Position} type={Type} value ={PelletValue}")]
    public class Cell
    {
        public Cell(int x, int y, CellType cellType)
        {
            Position = new Coordinate(x, y);
            Type = cellType;
            Neighbours = new Dictionary<Direction, Cell>();
            VisibleCells = new HashSet<Cell>();
            PelletValue = 1;
        }

        public int PelletValue { get; set; }

        public Coordinate Position { get; }

        public CellType Type { get; }

        public Dictionary<Direction, Cell> Neighbours { get; set; }

        public HashSet<Cell> VisibleCells { get; set; }

        public bool HasPellet => PelletValue > 0;

        protected bool Equals(Cell other)
        {
            return Equals(Position, other.Position);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((Cell)obj);
        }

        public override int GetHashCode()
        {
            return Position != null
                       ? Position.GetHashCode()
                       : 0;
        }
    }
}
namespace JoinThePac.Models
{
    public enum CellType
    {
        Unknown = 0,
        Floor = 1,
        Wall = 2

    }
}

namespace JoinThePac.Models
{
    [DebuggerDisplay("x = {X} y = {Y}")]
    public class Coordinate
    {
        public int X { get; private set; }

        public int Y { get; private set; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Coordinate(Coordinate position)
        {
            X = position.X;
            Y = position.Y;
        }

        public int Manhattan(Coordinate pos)
        {
            return Math.Abs(X - pos.X) + Math.Abs(Y - pos.Y);
        }

        public override string ToString()
        {
            return $"{X} {Y}";
        }

        public bool IsSame(Coordinate pos)
        {
            return X == pos.X && Y == pos.Y;
        }

        public void Update(Coordinate pos)
        {
            Update(pos.X, pos.Y);
        }

        public void Update(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
namespace JoinThePac.Models
{
    public enum Direction
    {
        Unknown,
        North,
        East,
        West,
        South
    }
}

namespace JoinThePac.Models
{
    public class Map
    {
        public int Width { get; }

        public int Height { get; }

        public Cell[,] Cells { get; }

        public HashSet<Cell> SuperPellets { get; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Cell[height, width];
            SuperPellets = new HashSet<Cell>();
        }

        public void Build(int row, string line)
        {
            for (var i = 0; i < line.Length; i++)
            {
                var ch = line[i];
                var cellType = CellType.Floor;
                if (ch == '#')
                {
                    cellType = CellType.Wall;
                }

                Cells[row, i] = new Cell(i, row, cellType);
            }
        }

        public void SetupCells()
        {
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    CheckAndAddNeighbour(Cells[i, j], j, i - 1, Direction.North);
                    CheckAndAddNeighbour(Cells[i, j], j - 1, i, Direction.East);
                    CheckAndAddNeighbour(Cells[i, j], j + 1, i, Direction.West);
                    CheckAndAddNeighbour(Cells[i, j], j, i + 1, Direction.South);
                }
            }

            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    AddVisibleCells(Cells[i, j], Direction.North);
                    AddVisibleCells(Cells[i, j], Direction.South);
                    AddVisibleCells(Cells[i, j], Direction.East);
                    AddVisibleCells(Cells[i, j], Direction.West);
                }
            }
        }

        private void AddVisibleCells(Cell cell, Direction direction)
        {
            var currentCell = cell;
            var count = 0;
            while (true)
            {
                if ((count > 0 && currentCell.Equals(cell)) || !currentCell.Neighbours.ContainsKey(direction))
                {
                    break;
                }
                else
                {
                    count++;
                    var neighbour = currentCell.Neighbours[direction];
                    cell.VisibleCells.Add(neighbour);
                    currentCell = neighbour;
                }
            }
        }

        private void CheckAndAddNeighbour(Cell cell, int x, int y, Direction direction)
        {
            if (x < 0)
            {
                x = Width - 1;
            }
            else if (x >= Width)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = Height - 1;
            }
            else if (y >= Height)
            {
                y = 0;
            }

            var cellType = Cells[y, x].Type;
            if (cellType == CellType.Floor)
            {
                cell.Neighbours.Add(direction, Cells[y, x]);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            //sb.Append("  |");
            //for (var i = 0; i < Width; i++)
            //{
            //    sb.Append($"{i:D2} ");
            //}

            //sb.AppendLine();
            //for (var i = 0; i < Width; i++)
            //{
            //    sb.Append($"---");
            //}
            //sb.AppendLine();
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    //if (j == 0)
                    //{
                    //    sb.Append($"{i:D2}|");
                    //}
                    if (Cells[i, j].Type == CellType.Floor)
                    {
                        //sb.Append($"{Cells[i,j].Neighbours.Count:D2} ");
                        sb.Append($"{Cells[i, j].Neighbours.Count}");
                    }
                    else if (Cells[i, j].Type == CellType.Wall)
                    {
                        //sb.Append("## ");
                        sb.Append("#");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public void SetCellValue(int x, int y, int pelletValue)
        {
            var cell = Cells[y, x];
            cell.PelletValue = pelletValue;
            if (pelletValue == 10)
            {
                SuperPellets.Add(cell);
            }
        }

        public void ResetSuperPellets()
        {
            SuperPellets.Clear();
        }
    }
}

namespace JoinThePac.Models
{
    [DebuggerDisplay("Id = {Id} Position = {Position} type={Type} isAlive={IsAlive} ")]
    public class Pac
    {
        public int Id { get; }

        public Coordinate Position { get; }

        private Coordinate _previousPosition;

        public bool IsAlive { get; private set; }

        public PacType Type { get; private set; }

        public int AbilityCooldown { get; private set; }

        public int SpeedTurnsLeft { get; private set; }

        public Pac(int id, int x, int y, PacType type)
        {
            Id = id;
            Type = type;
            Position = new Coordinate(x, y);
            IsAlive = true;
            _previousPosition = new Coordinate(-1, -1);
        }

        public bool IsInSamePosition()
        {
            return _previousPosition.IsSame(Position);
        }

        public void Update(int x, int y, PacType type, int speedTurnsLeft, int abilityCooldown)
        {
            _previousPosition = new Coordinate(Position);
            Position.Update(x, y);
            IsAlive = type != PacType.Unknown;
            Type = type;
            SpeedTurnsLeft = speedTurnsLeft;
            AbilityCooldown = abilityCooldown;
        }

        public void Reset()
        {
            IsAlive = false;
        }

        public bool CanBeEaten(PacType type)
        {
            switch (Type)
            {
                case PacType.Rock:
                    return type == PacType.Paper;
                case PacType.Paper:
                    return type == PacType.Scissors;
                case PacType.Scissors:
                    return type == PacType.Rock;
                default:
                    return false;
            }
        }
    }
}
namespace JoinThePac.Models
{
    public enum PacType
    {
        Unknown,

        Rock,

        Paper,

        Scissors
    }

    public static class PacTypeExtensions
    {
        public static PacType FromString(string type)
        {
            if (type == "ROCK")
            {
                return PacType.Rock;
            }

            if (type == "PAPER")
            {
                return PacType.Paper;
            }

            if (type == "SCISSORS")
            {
                return PacType.Scissors;
            }

            return PacType.Unknown;
        }
    }
}

namespace JoinThePac.Models
{
    public class Player
    {
        public Player()
        {
            Pacs = new Dictionary<int, Pac>();
        }

        public int Score { get; set; }

        public Dictionary<int, Pac> Pacs { get; }

        public void UpdatePac(
            int id,
            int x,
            int y,
            PacType type,
            int speedTurnsLeft,
            int abilityCooldown)
        {
            if (!Pacs.ContainsKey(id))
            {
                Pacs[id] = new Pac(id, x, y, type);
            }
            else
            {
                Pacs[id].Update(x, y, type, speedTurnsLeft, abilityCooldown);
            }
        }

        public void Reset()
        {
            foreach (var (_, pac) in Pacs)
            {
                pac.Reset();
            }
        }

        public void ResetVisibleCells(Map map)
        {
            foreach (var (_, pac) in Pacs)
            {
                var pacCell = map.Cells[pac.Position.Y, pac.Position.X];
                pacCell.PelletValue = 0;
                if (pac.IsAlive)
                {
                    foreach (var visibleCell in pacCell.VisibleCells)
                    {
                        visibleCell.PelletValue = 0;
                    }
                }
            }
        }
    }
}


namespace JoinThePac.Services
{
    public static class BFS
    {
        public static List<Cell> GetClosestCells(Cell from, Func<Cell, bool> condition, Func<Cell, bool> obstacleCondition, int maxLength)
        {
            var open = new List<Cell> { from };
            var seen = new HashSet<Cell> { from };
            var cells = new List<Cell>();

            while (open.Any() && cells.Count < maxLength)
            {
                var currentCell = open.First();
                open.RemoveAt(0);

                if (condition(currentCell))
                {
                    cells.Add(currentCell);
                }

                foreach (var (_, neighbour) in currentCell.Neighbours)
                {
                    if (seen.Add(neighbour) && !obstacleCondition(neighbour))
                    {
                        open.Add(neighbour);
                    }
                }
            }

            return cells;
        }

        public static List<Cell> GetPath(Cell from, Cell to, Func<Cell, bool> obstacleCondition)
        {
            if (obstacleCondition(to))
            {
                return null;
            }

            var fromCell = new BfsCell(null, from);
            var open = new List<BfsCell> { fromCell };
            var seen = new HashSet<Cell> { from };
            while (open.Any())
            {
                var currentCell = open.First();
                open.RemoveAt(0);

                if (currentCell.Cell.Equals(to))
                {
                    var list = new List<Cell>();
                    while (!currentCell.Cell.Equals(from))
                    {
                        list.Insert(0, currentCell.Cell);
                        currentCell = currentCell.Parent;
                    }

                    return list;
                }

                foreach (var (_, neighbour) in currentCell.Cell.Neighbours)
                {
                    if (seen.Add(neighbour) && !obstacleCondition(neighbour))
                    {
                        open.Add(new BfsCell(currentCell, neighbour));
                    }
                }
            }

            return null;
        }
    }

    public class BfsCell
    {
        public BfsCell(BfsCell parent, Cell cell)
        {
            Parent = parent;
            Cell = cell;
        }

        public BfsCell Parent { get; }

        public Cell Cell { get; }
    }
}

namespace JoinThePac.Services
{
    public static class Io
    {
        private static StreamReader _file;

        public static void Initialize()
        {
            if (Constants.IsLocalRun)
            {
                _file = new StreamReader(@".\in.txt");
            }
        }

        public static void Debug(string output)
        {
            if (Constants.IsDebugOn || Constants.IsForInput)
            {
                Console.Error.WriteLine(output);
            }
        }

        public static void WriteLine(string output)
        {
            Console.WriteLine(output);
        }

        public static string ReadLine()
        {
            if (Constants.IsLocalRun)
            {
                return _file.ReadLine();
            }
            else
            {
                var input = Console.ReadLine();

                if (Constants.IsForInput)
                {
                    Debug("IN");
                    Debug(input);
                    Debug("/IN");
                }
                else
                {
                    Debug(input);
                }

                return input;
            }
        }
    }
}