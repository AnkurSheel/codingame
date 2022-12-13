using System;
using System.Collections.Generic;
using FallChallenge2022.Common.Services;
using FallChallenge2022.Models;
using System.Linq;
using FallChallenge2022.Agent;
using SpringChallenge2021.Actions;
using FallChallenge2022.Action;
using System.IO;


 // 14/12/2022 01:04


namespace FallChallenge2022
{
    public static class Constants
    {
        public static readonly Random RandomGenerator = new Random();
        
    }
}

namespace FallChallenge2022
{
    public class Game
    {
        private readonly Tile[,] _board;

        public int Width { get; }

        public int Height { get; }

        public Player MyPlayer { get; }

        public Game()
        {
            var inputs = Io.ReadLine().Split(' ');
            Width = int.Parse(inputs[0]);
            Height = int.Parse(inputs[1]);
            MyPlayer = new Player();

            _board = new Tile[Width, Height];
        }

        public void Parse()
        {
            var inputs = Io.ReadLine().Split(' ');
            var myMatter = int.Parse(inputs[0]);
            var oppMatter = int.Parse(inputs[1]);

            var myUnits = new List<Unit>();

            var myTiles = new List<Position>();
            
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    var tile = new Tile(j, i);

                    _board[j, i] = tile;

                    for (var k = 0; k < tile.NumberOfUnits; k++)
                    {
                        if (tile.Owner == 1)
                        {
                            myTiles.Add(tile.Position);
                            var unit = new Unit(tile);

                            myUnits.Add(unit);
                        }
                    }
                }
            }

            MyPlayer.ReInit(myMatter, myUnits, myTiles);
        }

        public Tile? GetTileAt(Position position)
        {
            if (position.X >= 0 && position.X < Width && position.Y >= 0 && position.Y < Height)
            {
                return _board[position.X, position.Y];
            }

            return null;
        }
    }
}

namespace FallChallenge2022
{
    public class Player
    {
        public int Matter { get; private set; }

        public IReadOnlyList<Unit> Units { get; private set; }

        public List<Position> Tiles { get; private set; }

        public void ReInit(int matter, IReadOnlyList<Unit> units, List<Position> tiles)
        {
            Matter = matter;
            Units = units;
            Tiles = tiles;
            Io.Debug($"Tiles {tiles.Count}");
        }

        public bool CanSpawn()
        {
            return Matter > 10;
        }

        public Position GetRandomTilePosition()
        {
            return Tiles[Constants.RandomGenerator.Next(Tiles.Count)];
        }
    }
}

namespace FallChallenge2022
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var game = new Game();
            IAgent agent = new SimpleAgentV1();

            // game loop
            while (true)
            {
                game.Parse();

                var actions = agent.GetActions(game);
                var output = string.Join(";", actions.Select(x => x.GetOutputAction()));
                Io.WriteLine(output);
            }
        }
    }
}
namespace SpringChallenge2021.Actions
{
    public interface IAction
    {
        string GetOutputAction();
    }
}

namespace FallChallenge2022.Action
{
    public class MoveAction : IAction
    {
        private readonly Position _from;
        private readonly Position _to;

        public MoveAction(
            Position from,
            Position to)
        {
            _from = from;
            _to = to;
        }

        public string GetOutputAction()
            => $"MOVE 1 {_from.X} {_from.Y} {_to.X} {_to.Y}";
    }
}

namespace FallChallenge2022.Action
{
    public class SpawnAction : IAction
    {
        private readonly Position _position;

        public SpawnAction(Position position)
        {
            _position = position;
        }

        public string GetOutputAction()
            => $"SPAWN 1 {_position.X} {_position.Y}";
    }
}
namespace SpringChallenge2021.Actions
{
    public class WaitAction : IAction
    {
        public string GetOutputAction()
        {
            return "WAIT";
        }
    }
}

namespace FallChallenge2022.Agent
{
    public interface IAgent
    {
        IReadOnlyList<IAction> GetActions(Game game);
    }
}

namespace FallChallenge2022.Agent
{
    public class SimpleAgentV1 : IAgent
    {
        public IReadOnlyList<IAction> GetActions(Game game)
        {
            var actions = new List<IAction>();
            Io.Debug(game.Width.ToString());
            Io.Debug(game.Height.ToString());

            var alreadyTargetedPositions = new HashSet<Position>();

            var availableMatter = game.MyPlayer.Matter;

            while (availableMatter >= 10)
            {
                actions.Add(new SpawnAction(game.MyPlayer.GetRandomTilePosition()));
                availableMatter -= 10;
            }

            foreach (var unit in game.MyPlayer.Units)
            {
                var action = GetMoveActionForUnit(game, unit, alreadyTargetedPositions);

                if (action != null)
                {
                    actions.Add(action);
                }
            }

            if (actions.Count == 0)
            {
                actions.Add(new WaitAction());
            }

            return actions;
        }

        private IAction GetMoveActionForUnit(Game game, Unit unit, HashSet<Position> alreadyTargetedPositions)
        {
            var newPosition = TryGetValidPosition(game, new Position(unit.Tile.Position.X - 1, unit.Tile.Position.Y), alreadyTargetedPositions);

            if (newPosition != null)
            {
                alreadyTargetedPositions.Add(newPosition);
                return new MoveAction(unit.Tile.Position, newPosition);
            }

            newPosition = TryGetValidPosition(game, new Position(unit.Tile.Position.X + 1, unit.Tile.Position.Y), alreadyTargetedPositions);

            if (newPosition != null)
            {
                alreadyTargetedPositions.Add(newPosition);
                return new MoveAction(unit.Tile.Position, newPosition);
            }

            newPosition = TryGetValidPosition(game, new Position(unit.Tile.Position.X, unit.Tile.Position.Y - 1), alreadyTargetedPositions);

            if (newPosition != null)
            {
                alreadyTargetedPositions.Add(newPosition);
                return new MoveAction(unit.Tile.Position, newPosition);
            }

            newPosition = TryGetValidPosition(game, new Position(unit.Tile.Position.X, unit.Tile.Position.Y + 1), alreadyTargetedPositions);

            if (newPosition != null)
            {
                alreadyTargetedPositions.Add(newPosition);
                return new MoveAction(unit.Tile.Position, newPosition);
            }

            var middlePosition = new Position(game.Width / 2, game.Height / 2);

            if (middlePosition == unit.Tile.Position)
            {
                return new MoveAction(unit.Tile.Position, new Position(Constants.RandomGenerator.Next(game.Width - 1), Constants.RandomGenerator.Next(game.Height - 1)));
            }
            else
            {
                return new MoveAction(unit.Tile.Position, middlePosition);
            }
        }

        private Position? TryGetValidPosition(Game game, Position to, HashSet<Position> alreadyTargetedTiles)
        {
            var newTile = game.GetTileAt(to);

            return newTile != null && newTile.Owner != 1 && newTile.ScrapAmount > 0 && !alreadyTargetedTiles.Contains(newTile.Position)
                ? newTile.Position
                : null;
        }
    }
}
namespace FallChallenge2022.Common
{
    public static class Constants
    {
        public const bool IsDebugOn = false;

        public const bool IsForInput = false;

        public const bool IsLocalRun = false;

        public const bool ShowInput = false;
    }
}
namespace FallChallenge2022.Models
{
    public record Position(int X, int Y);
}

namespace FallChallenge2022.Models
{
    public class Tile
    {
        public int ScrapAmount { get; set; }

        public int Owner { get; set; }

        public int NumberOfUnits { get; set; }

        public Position Position { get; }

        private int _recycler;
        private int _canBuild;
        private int _canSpawn;
        private int _inRangeOfRecycler;

        public Tile(int posX, int posY)
        {
            Position = new Position(posX, posY);
            
            var inputs = Io.ReadLine().Split(' ');
            ScrapAmount = int.Parse(inputs[0]);
            Owner = int.Parse(inputs[1]); // 1 = me, 0 = foe, -1 = neutral
            NumberOfUnits = int.Parse(inputs[2]);
            _recycler = int.Parse(inputs[3]);
            _canBuild = int.Parse(inputs[4]);
            _canSpawn = int.Parse(inputs[5]);
            _inRangeOfRecycler = int.Parse(inputs[6]);
        }
    }
}
namespace FallChallenge2022.Models
{
    public record Unit(Tile Tile);
}

namespace FallChallenge2022.Common.Services
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
            if (Constants.IsDebugOn || Constants.IsForInput || Constants.ShowInput)
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
                else if(Constants.ShowInput)
                {
                    Debug(input);
                }

                return input;
            }
        }
    }
}