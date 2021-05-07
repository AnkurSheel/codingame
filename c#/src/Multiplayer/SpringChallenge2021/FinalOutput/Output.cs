


using SpringChallenge2021.Actions;
using System.Collections.Generic;
using System.Linq;
using SpringChallenge2021.Common.Services;
using SpringChallenge2021.Models;
using System;
using System.IO;


 // 07/05/2021 11:35


namespace SpringChallenge2021
{
    internal class Action
    {
        private const string WAIT = "WAIT";
        private const string SEED = "SEED";
        private const string GROW = "GROW";
        private const string COMPLETE = "COMPLETE";

        public static IAction Parse(string action)
        {
            var parts = action.Split(" ");
            switch (parts[0])
            {
                case GROW:
                    return new GrowAction(int.Parse(parts[1]));
                case COMPLETE:
                    return new CompleteAction(int.Parse(parts[1]));
                case WAIT:
                case SEED:
                default:
                    return new WaitAction();
            }
        }

        private readonly string _type;
        private readonly int _targetCellIdx;
        private readonly int _sourceCellIdx;

        public Action(string type, int sourceCellIdx, int targetCellIdx)
        {
            _type = type;
            _targetCellIdx = targetCellIdx;
            _sourceCellIdx = sourceCellIdx;
        }

        public Action(string type, int targetCellIdx)
            : this(type, 0, targetCellIdx)
        {
        }

        public Action(string type)
            : this(type, 0, 0)
        {
        }

        public override string ToString()
        {
            if (_type == WAIT)
            {
                return WAIT;
            }

            if (_type == SEED)
            {
                return $"{SEED} {_sourceCellIdx} {_targetCellIdx}";
            }

            return $"{_type} {_targetCellIdx}";
        }
    }
}

namespace SpringChallenge2021
{
    internal class Game
    {
        private int _day;
        private int _nutrients;
        private readonly List<Cell> _board;
        private readonly List<IAction> _possibleActions;
        private readonly List<Tree> _trees;
        private readonly Player _myPlayer;
        private readonly Player _opponentPlayer;

        public Game()
        {
            _board = new List<Cell>();
            _possibleActions = new List<IAction>();
            _trees = new List<Tree>();
            _myPlayer = new Player();
            _opponentPlayer = new Player();

            GenerateBoard();
        }

        public void ReadGameState()
        {
            _day = int.Parse(Io.ReadLine()); // the game lasts 24 days: 0-23
            _nutrients = int.Parse(Io.ReadLine()); // the base score you gain from the next COMPLETE action

            _myPlayer.Parse();
            _opponentPlayer.Parse();

            ReadTrees();
            ReadPossibleActions();
        }

        public IAction GetNextAction()
        {
            var completeAction = _possibleActions.FirstOrDefault(x => x is CompleteAction);
            if (completeAction != null)
            {
                return completeAction;
            }

            var growAction = _possibleActions.FirstOrDefault(x => x is GrowAction);
            if (growAction != null)
            {
                return growAction;
            }

            return new WaitAction();
        }

        private void GenerateBoard()
        {
            var numberOfCells = int.Parse(Io.ReadLine()); // 37
            for (var i = 0; i < numberOfCells; i++)
            {
                var inputs = Io.ReadLine().Split(' ');
                var index = int.Parse(inputs[0]); // 0 is the center cell, the next cells spiral outwards
                var richness = int.Parse(inputs[1]); // 0 if the cell is unusable, 1-3 for usable cells
                var neigh0 = int.Parse(inputs[2]); // the index of the neighbouring cell for each direction
                var neigh1 = int.Parse(inputs[3]);
                var neigh2 = int.Parse(inputs[4]);
                var neigh3 = int.Parse(inputs[5]);
                var neigh4 = int.Parse(inputs[6]);
                var neigh5 = int.Parse(inputs[7]);
                int[] neighs = {neigh0, neigh1, neigh2, neigh3, neigh4, neigh5};
                var cell = new Cell(index, (SoilQuality) richness, neighs);
                _board.Add(cell);
            }
        }

        private void ReadTrees()
        {
            Io.Debug("Reading Trees");

            _trees.Clear();
            var numberOfTrees = int.Parse(Io.ReadLine()); // the current amount of trees
            for (var i = 0; i < numberOfTrees; i++)
            {
                var inputs = Io.ReadLine().Split(' ');
                var cellIndex = int.Parse(inputs[0]); // location of this tree
                var size = int.Parse(inputs[1]); // size of this tree: 0-3
                var isMine = inputs[2] != "0"; // 1 if this is your tree
                var isDormant = inputs[3] != "0"; // 1 if this tree is dormant
                var tree = new Tree(cellIndex, (TreeSize) size, isMine, isDormant);
                _trees.Add(tree);
            }
        }

        private void ReadPossibleActions()
        {
            Io.Debug("Getting next action");

            _possibleActions.Clear();
            var numberOfPossibleMoves = int.Parse(Io.ReadLine());
            for (var i = 0; i < numberOfPossibleMoves; i++)
            {
                var possibleMove = Io.ReadLine();
                _possibleActions.Add(Action.Parse(possibleMove));
            }
        }
    }
}

namespace SpringChallenge2021
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Io.Initialize();

            var game = new Game();


            // game loop
            while (true)
            {
                game.ReadGameState();

                var action = game.GetNextAction();
                Io.WriteLine(action.GetOutputAction());
            }
        }
    }
}
namespace SpringChallenge2021.Actions
{
    public class CompleteAction : IAction
    {
        private readonly int _index;

        public CompleteAction(int index)
        {
            _index = index;
        }

        public string GetOutputAction()
        {
            return $"COMPLETE {_index}";
        }
    }
}
namespace SpringChallenge2021.Actions
{
    public class GrowAction : IAction
    {
        private readonly int _index;

        public GrowAction(int index)
        {
            _index = index;
        }

        public string GetOutputAction()
        {
            return $"GROW {_index}";
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
namespace SpringChallenge2021.Common
{
    public static class Constants
    {
        public const bool IsDebugOn = true;

        public const bool IsForInput = false;

        public const bool IsLocalRun = false;
    }
}
namespace SpringChallenge2021.Models
{
    internal class Cell
    {
        private int _index;
        private SoilQuality _soilQuality;
        private int[] _neighbours;

        public Cell(int index, SoilQuality soilQuality, int[] neighbours)
        {
            _index = index;
            _soilQuality = soilQuality;
            _neighbours = neighbours;
        }
    }
}

namespace SpringChallenge2021.Models
{
    public class Player
    {
        private int _sunPoints;
        private int _score;
        private bool _isWaiting;

        public void Parse()
        {
            Io.Debug("Reading Player State");
            var inputs = Io.ReadLine().Split(' ');

            _sunPoints = int.Parse(inputs[0]);
            _score = int.Parse(inputs[1]); // your current score
            _isWaiting = false;

            if (inputs.Length == 3)
            {
                _isWaiting = inputs[2] != "0"; // whether your opponent is asleep until the next day
            }
        }
    }
}
namespace SpringChallenge2021.Models
{
    public enum SoilQuality
    {
        Unknown = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }
}
namespace SpringChallenge2021.Models
{
    public class Tree
    {
        private int _cellIndex;
        private TreeSize _size;
        private bool _isMine;
        private bool _isDormant;

        public Tree(int cellIndex, TreeSize size, bool isMine, bool isDormant)
        {
            _cellIndex = cellIndex;
            _size = size;
            _isMine = isMine;
            _isDormant = isDormant;
        }
    }
}
namespace SpringChallenge2021.Models
{
    public enum TreeSize
    {
        Unknown = 0,
        Small = 1,
        Medium = 2,
        Lage = 3
    }
}

namespace SpringChallenge2021.Common.Services
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