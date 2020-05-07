using System;
using JoinThePac.Agents;
using JoinThePac.Models;
using JoinThePac.Services;
using System.Collections.Generic;
using System.Text;
using System.IO;


 // 08/05/2020 07:26


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
    /**
 * Grab the pellets as fast as you can!
 **/
    internal class JoinThePac
    {
        private static void Main(string[] args)
        {
            string[] inputs;
            inputs = Io.ReadLine().Split(' ');
            var width = int.Parse(inputs[0]); // size of the grid
            var height = int.Parse(inputs[1]); // top left corner is (x=0, y=0)

            var map = new Map(width, height);

            for (var i = 0; i < height; i++)
            {
                var row = Io.ReadLine(); // one line of the grid: space " " is floor, pound "#" is wall
                map.Build(i, row);
            }

            map.SetupCells();
            //Io.Debug($"Built Map{Environment.NewLine}{map}");

            var myPlayer = new Player();
            // game loop
            while (true)
            {
                inputs = Io.ReadLine().Split(' ');
                myPlayer.Score = int.Parse(inputs[0]);

                var opponentScore = int.Parse(inputs[1]);
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
                    if (mine)
                    {
                        myPlayer.Pac = new Pac(pacId, x, y);
                    }
                }

                map.ClearPellets();
                var visiblePelletCount = int.Parse(Io.ReadLine()); // all pellets in sight
                for (var i = 0; i < visiblePelletCount; i++)
                {
                    inputs = Io.ReadLine().Split(' ');
                    var x = int.Parse(inputs[0]);
                    var y = int.Parse(inputs[1]);
                    var value = int.Parse(inputs[2]); // amount of points this pellet is worth
                    map.Cells[y, x].Pellet = value;
                }

                // Write an action using Console.WriteLine()
                // To debug: Console.Error.WriteLine("Debug messages...");
                var agent = new ReactAgent(map, myPlayer);
                Io.WriteLine(agent.GetAction()); // MOVE <pacId> <x> <y>
            }
        }
    }
}

namespace JoinThePac.Agents
{
    public class ReactAgent
    {
        private readonly Map _map;

        private readonly Player _myPlayer;

        public ReactAgent(Map map, Player myPlayer)
        {
            _map = map;
            _myPlayer = myPlayer;
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
            return $"MOVE {pac.Id} {_map.Width/2} {_map.Height/2}";
        }
    }
}

namespace JoinThePac.Models
{
    public class Cell
    {
        public Cell(int x, int y, CellType cellType)
        {
            X = x;
            Y = y;
            Type = cellType;
            Neighbours = new Dictionary<Direction, Cell>();
            Pellet = 0;
        }

        public int X { get; }

        public int Y { get; }

        public CellType Type { get; }

        public Dictionary<Direction, Cell> Neighbours { get; set; }

        public int Pellet { get; set; }

        public bool HasPellet => Pellet > 0;
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

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Cell[height, width];
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
        }

        private void CheckAndAddNeighbour(Cell cell, int x, int y, Direction direction)
        {
            if (IsValid(x, y))
            {
                cell.Neighbours.Add(direction, Cells[y, x]);
            }
        }

        private bool IsValid(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return false;
            }

            var cellType = Cells[y, x].Type;
            return cellType == CellType.Floor;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    if (Cells[i, j].Type == CellType.Floor)
                    {
                        sb.Append($"{Cells[i,j].Neighbours.Count}");
                    }
                    else if (Cells[i, j].Type == CellType.Wall)
                    {
                        sb.Append('#');
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public void ClearPellets()
        {
            foreach (var cell in Cells)
            {
                cell.Pellet = 0;
            }
        }
    }
}
namespace JoinThePac.Models
{
    public class Pac
    {
        public int Id { get; }

        public int X { get; }

        public int Y { get; }

        public Pac(int id, int x, int y)
        {
            Id = id;
            X = x;
            Y = y;
        }
    }
}
namespace JoinThePac.Models
{
    public class Player
    {
        public int Score { get; set; }

        public Pac Pac { get; set; }
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
            if (Constants.IsDebugOn)
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