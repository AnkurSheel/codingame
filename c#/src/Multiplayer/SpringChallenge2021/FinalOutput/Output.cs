using System.Collections.Generic;
using SpringChallenge2021.Actions;
using SpringChallenge2021.Common.Services;
using SpringChallenge2021.Models;
using SpringChallenge2021.Agents;
using System.Linq;
using System;
using System.IO;


 // 12/05/2021 11:39

namespace SpringChallenge2021
{
    public class Constants
    {
        public const int DayCutOff = 21;
        public const int MaxDays = 24;
        public const int MaxLargeTreesToKeep = 7;
    }
}

namespace SpringChallenge2021
{
    public class Game
    {
        private int _nutrients;
        private readonly Player _opponentPlayer;

        public List<IAction> PossibleActions { get; }

        public Dictionary<int, Cell> Board { get; }

        public Dictionary<int, Tree> Trees { get; }

        public Player MyPlayer { get; }

        public int Day { get; private set; }

        public HexDirection SunDirection { get; private set; }

        public Dictionary<Cell, TreeSize> Shadows { get; }

        public Dictionary<Cell, TreeSize> ShadowsNextDay { get; }

        public Game()
        {
            Board = new Dictionary<int, Cell>();
            PossibleActions = new List<IAction>();
            Trees = new Dictionary<int, Tree>();
            MyPlayer = new Player();
            _opponentPlayer = new Player();

            GenerateBoard();
            Shadows = new Dictionary<Cell, TreeSize>();
            ShadowsNextDay = new Dictionary<Cell, TreeSize>();
        }

        public void ReInit()
        {
            Trees.Clear();
            PossibleActions.Clear();
            Shadows.Clear();
            ShadowsNextDay.Clear();

            MyPlayer.ReInit();
            _opponentPlayer.ReInit();

            ReadGameState();

            SunDirection = (HexDirection) (Day % 6);
            SetupShadowsForNextDay();
        }

        private void ReadGameState()
        {
            Day = int.Parse(Io.ReadLine()); // the game lasts 24 days: 0-23
            _nutrients = int.Parse(Io.ReadLine()); // the base score you gain from the next COMPLETE action

            MyPlayer.Parse();
            _opponentPlayer.Parse();

            ReadTrees();
            ReadPossibleActions();
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
                Board.Add(index, cell);
            }

            foreach (var (_, cell) in Board)
            {
                cell.UpdateNeighbours(Board);
            }
        }

        private void ReadTrees()
        {
            Io.Debug("Reading Trees");

            var numberOfTrees = int.Parse(Io.ReadLine()); // the current amount of trees
            for (var i = 0; i < numberOfTrees; i++)
            {
                var inputs = Io.ReadLine().Split(' ');
                var cellIndex = int.Parse(inputs[0]); // location of this tree
                var size = int.Parse(inputs[1]); // size of this tree: 0-3
                var isMine = inputs[2] != "0"; // 1 if this is your tree
                var isDormant = inputs[3] != "0"; // 1 if this tree is dormant
                var tree = new Tree(cellIndex, (TreeSize) size, isMine, isDormant);
                Trees.Add(cellIndex, tree);
                if (isMine)
                {
                    MyPlayer.AddTree(tree);
                }
                else
                {
                    _opponentPlayer.AddTree(tree);
                }
            }
        }

        private void ReadPossibleActions()
        {
            Io.Debug("Getting possible actions");

            var numberOfPossibleMoves = int.Parse(Io.ReadLine());
            for (var i = 0; i < numberOfPossibleMoves; i++)
            {
                var possibleMove = Io.ReadLine();

                var parts = possibleMove.Split(" ");
                IAction action = parts[0] switch
                {
                    "SEED" => new SeedAction(int.Parse(parts[1]), int.Parse(parts[2])),
                    "GROW" => new GrowAction(int.Parse(parts[1])),
                    "COMPLETE" => new CompleteAction(int.Parse(parts[1])),
                    _ => new WaitAction()
                };

                PossibleActions.Add(action);
            }
        }

        private void SetupShadows()
        {
            GetShadows(SunDirection, Shadows);
        }

        private void SetupShadowsForNextDay()
        {
            var sunDirection = (HexDirection) (((int) SunDirection + 1) % 6);
            GetShadows(sunDirection, ShadowsNextDay);
        }

        private void GetShadows(HexDirection sunDirection, IDictionary<Cell, TreeSize> shadows)
        {
            foreach (var (treeIndex, tree) in Trees)
            {
                var cell = Board[treeIndex];
                for (var i = 0; i < (int) tree.Size && cell != null; i++)
                {
                    var neighbour = cell.Neighbours[sunDirection];
                    if (neighbour != null)
                    {
                        if (shadows.ContainsKey(neighbour))
                        {
                            if (shadows[neighbour] > tree.Size)
                            {
                                shadows[neighbour] = tree.Size;
                            }
                        }
                        else
                        {
                            shadows.Add(neighbour, tree.Size);
                        }
                    }

                    cell = neighbour;
                }
            }
        }
    }
}

namespace SpringChallenge2021
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Io.Initialize();

            var game = new Game();
            var agent = new SimpleAgent();

            // game loop
            while (true)
            {
                game.ReInit();

                var action = agent.GetAction(game);
                Io.WriteLine(action.GetOutputAction());
            }
        }
    }
}
namespace SpringChallenge2021.Actions
{
    public class CompleteAction : IAction
    {
        public int Index { get; }

        public CompleteAction(int index)
        {
            Index = index;
        }

        public string GetOutputAction()
        {
            return $"COMPLETE {Index}";
        }
    }
}
namespace SpringChallenge2021.Actions
{
    public class GrowAction : IAction
    {
        public int Index { get; }

        public GrowAction(int index)
        {
            Index = index;
        }

        public string GetOutputAction()
        {
            return $"GROW {Index}";
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
    public class SeedAction : IAction
    {
        private readonly int _srcTreeIndex;
        public int SeedIndex { get; }

        public SeedAction(int srcTreeIndex, int seedIndex)
        {
            _srcTreeIndex = srcTreeIndex;
            SeedIndex = seedIndex;
        }

        public string GetOutputAction()
        {
            return $"SEED {_srcTreeIndex} {SeedIndex}";
        }
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

namespace SpringChallenge2021.Agents
{
    public class SimpleAgent
    {
        public IAction GetAction(Game game)
        {
            var completeAction = GetBestCompleteAction(game);
            if (completeAction != null)
            {
                return completeAction;
            }

            if (game.Day <= Constants.DayCutOff || (!game.MyPlayer.Trees[TreeSize.Large].Any()))
            {
                var growAction = GetBestGrowAction(game);
                if (growAction != null)
                {
                    return growAction;
                }
            }

            if (game.Day <= Constants.DayCutOff)
            {
                var seedAction = GetBestSeedAction(game);
                if (seedAction != null)
                {
                    return seedAction;
                }
            }

            return new WaitAction();
        }

        private static IAction? GetBestCompleteAction(Game game)
        {
            var completeActions = game.PossibleActions.OfType<CompleteAction>().ToList();

            if (!completeActions.Any()
                || game.MyPlayer.Trees[TreeSize.Large].Count < Constants.MaxLargeTreesToKeep
                && game.Day < Constants.DayCutOff)
            {
                return null;
            }

            var bestCompleteAction = completeActions.FirstOrDefault();
            var bestSoilQuality = game.Board[bestCompleteAction.Index].SoilQuality;
            foreach (var completeAction in completeActions)
            {
                var cellSoilQuality = game.Board[completeAction.Index].SoilQuality;
                if (bestSoilQuality < cellSoilQuality)
                {
                    bestSoilQuality = cellSoilQuality;
                    bestCompleteAction = completeAction;
                }
            }

            return bestCompleteAction;
        }

        private static IAction? GetBestGrowAction(Game game)
        {
            var growActions = game.PossibleActions.OfType<GrowAction>().ToList();
            var bestSoilQuality = SoilQuality.Unusable;
            var bestTreeSize = TreeSize.Seed;
            var bestGrowAction = growActions.FirstOrDefault();
            foreach (var growAction in growActions)
            {
                var cell = game.Board[growAction.Index];
                if (game.ShadowsNextDay.ContainsKey(cell))
                {
                    var sizeOfTreeCastingShadow = game.ShadowsNextDay[cell];
                    var sizeOfTreeAfterGrowth = game.Trees[growAction.Index].Size + 1;
                    if (sizeOfTreeAfterGrowth <= sizeOfTreeCastingShadow)
                    {
                        continue;
                    }
                }

                var cellTreeSize = game.Trees[growAction.Index].Size;

                if (bestTreeSize < cellTreeSize)
                {
                    bestTreeSize = cellTreeSize;
                    bestGrowAction = growAction;
                    bestSoilQuality = cell.SoilQuality;
                }
                else if (bestTreeSize == cellTreeSize)
                {
                    var cellSoilQuality = cell.SoilQuality;
                    if (bestSoilQuality < cellSoilQuality)
                    {
                        bestSoilQuality = cellSoilQuality;
                        bestGrowAction = growAction;
                    }
                }
            }

            return bestGrowAction;
        }

        private IAction? GetBestSeedAction(Game game)
        {
            if (game.MyPlayer.Trees[TreeSize.Seed].Any())
            {
                return null;
            }

            var seedActions = game.PossibleActions.OfType<SeedAction>().ToList();
            var bestSoilQuality = SoilQuality.Unusable;
            var bestSeedAction = seedActions.FirstOrDefault();
            foreach (var seedAction in seedActions)
            {
                var cellSoilQuality = game.Board[seedAction.SeedIndex].SoilQuality;
                if (bestSoilQuality < cellSoilQuality)
                {
                    bestSoilQuality = cellSoilQuality;
                    bestSeedAction = seedAction;
                }
            }

            return bestSeedAction;
        }
    }
}

namespace SpringChallenge2021.Agents
{
    public class SimpleAgentV1
    {
        public IAction GetAction(Game game)
        {
            var completeAction = game.PossibleActions.FirstOrDefault(x => x is CompleteAction);
            if (completeAction != null)
            {
                return completeAction;
            }

            var growAction = game.PossibleActions.FirstOrDefault(x => x is GrowAction);
            if (growAction != null)
            {
                return growAction;
            }

            var seedAction = game.PossibleActions.FirstOrDefault(x => x is SeedAction);
            if (seedAction != null)
            {
                return seedAction;
            }

            return new WaitAction();
        }
    }
}

namespace SpringChallenge2021.Agents
{
    public class SimpleAgentV2
    {
        public IAction GetAction(Game game)
        {
            var completeAction = GetBestCompleteAction(game);
            if (completeAction != null)
            {
                return completeAction;
            }

            if (game.Day < Constants.DayCutOff + 1)
            {
                var growAction = GetBestGrowAction(game);
                if (growAction != null)
                {
                    return growAction;
                }

                var seedAction = GetBestSeedAction(game);
                if (seedAction != null)
                {
                    return seedAction;
                }
            }

            return new WaitAction();
        }

        private static IAction? GetBestCompleteAction(Game game)
        {
            if (game.MyPlayer.Trees[TreeSize.Large].Count < 3 && game.Day < Constants.DayCutOff)
            {
                return null;
            }

            var completeActions = game.PossibleActions.OfType<CompleteAction>().ToList();
            var bestCompleteAction = completeActions.FirstOrDefault();
            var bestSoilQuality = SoilQuality.Unusable;
            foreach (var completeAction in completeActions)
            {
                var cellSoilQuality = game.Board[completeAction.Index].SoilQuality;
                if (bestSoilQuality < cellSoilQuality)
                {
                    bestSoilQuality = cellSoilQuality;
                    bestCompleteAction = completeAction;
                }
            }

            return bestCompleteAction;
        }

        private static IAction? GetBestGrowAction(Game game)
        {
            var growActions = game.PossibleActions.OfType<GrowAction>().ToList();
            var bestSoilQuality = SoilQuality.Unusable;
            var bestTreeSize = TreeSize.Seed;
            var bestGrowAction = growActions.FirstOrDefault();
            foreach (var growAction in growActions)
            {
                var cellTreeSize = game.Trees[growAction.Index].Size;

                if (bestTreeSize < cellTreeSize)
                {
                    bestTreeSize = cellTreeSize;
                    bestGrowAction = growAction;
                    bestSoilQuality = game.Board[growAction.Index].SoilQuality;
                }
                else if (bestTreeSize == cellTreeSize)
                {
                    var cellSoilQuality = game.Board[growAction.Index].SoilQuality;
                    if (bestSoilQuality < cellSoilQuality)
                    {
                        bestSoilQuality = cellSoilQuality;
                        bestGrowAction = growAction;
                    }
                }
            }

            return bestGrowAction;
        }

        private IAction? GetBestSeedAction(Game game)
        {
            if (game.MyPlayer.Trees[TreeSize.Seed].Any())
            {
                return null;
            }

            var seedActions = game.PossibleActions.OfType<SeedAction>().ToList();
            var bestSoilQuality = SoilQuality.Unusable;
            var bestSeedAction = seedActions.FirstOrDefault();
            foreach (var seedAction in seedActions)
            {
                var cellSoilQuality = game.Board[seedAction.SeedIndex].SoilQuality;
                if (bestSoilQuality < cellSoilQuality)
                {
                    bestSoilQuality = cellSoilQuality;
                    bestSeedAction = seedAction;
                }
            }

            return bestSeedAction;
        }
    }
}

namespace SpringChallenge2021.Agents
{
    public class SimpleAgentV3
    {
        public IAction GetAction(Game game)
        {
            var completeAction = GetBestCompleteAction(game);
            if (completeAction != null)
            {
                return completeAction;
            }

            if (game.Day <= Constants.DayCutOff || (!game.MyPlayer.Trees[TreeSize.Large].Any()))
            {
                var growAction = GetBestGrowAction(game);
                if (growAction != null)
                {
                    return growAction;
                }
            }

            if (game.Day <= Constants.DayCutOff)
            {
                var seedAction = GetBestSeedAction(game);
                if (seedAction != null)
                {
                    return seedAction;
                }
            }

            return new WaitAction();
        }

        private static IAction? GetBestCompleteAction(Game game)
        {
            var completeActions = game.PossibleActions.OfType<CompleteAction>().ToList();

            if (!completeActions.Any()
                || game.MyPlayer.Trees[TreeSize.Large].Count < Constants.MaxLargeTreesToKeep
                && game.Day < Constants.DayCutOff)
            {
                return null;
            }

            var bestCompleteAction = completeActions.FirstOrDefault();
            var bestSoilQuality = game.Board[bestCompleteAction.Index].SoilQuality;
            foreach (var completeAction in completeActions)
            {
                var cellSoilQuality = game.Board[completeAction.Index].SoilQuality;
                if (bestSoilQuality < cellSoilQuality)
                {
                    bestSoilQuality = cellSoilQuality;
                    bestCompleteAction = completeAction;
                }
            }

            return bestCompleteAction;
        }

        private static IAction? GetBestGrowAction(Game game)
        {
            var growActions = game.PossibleActions.OfType<GrowAction>().ToList();
            var bestSoilQuality = SoilQuality.Unusable;
            var bestTreeSize = TreeSize.Seed;
            var bestGrowAction = growActions.FirstOrDefault();
            foreach (var growAction in growActions)
            {
                var cell = game.Board[growAction.Index];
                if (game.ShadowsNextDay.ContainsKey(cell))
                {
                    var sizeOfTreeCastingShadow = game.ShadowsNextDay[cell];
                    var sizeOfTreeAfterGrowth = game.Trees[growAction.Index].Size + 1;
                    if (sizeOfTreeAfterGrowth <= sizeOfTreeCastingShadow)
                    {
                        continue;
                    }
                }

                var cellTreeSize = game.Trees[growAction.Index].Size;

                if (bestTreeSize < cellTreeSize)
                {
                    bestTreeSize = cellTreeSize;
                    bestGrowAction = growAction;
                    bestSoilQuality = cell.SoilQuality;
                }
                else if (bestTreeSize == cellTreeSize)
                {
                    var cellSoilQuality = cell.SoilQuality;
                    if (bestSoilQuality < cellSoilQuality)
                    {
                        bestSoilQuality = cellSoilQuality;
                        bestGrowAction = growAction;
                    }
                }
            }

            return bestGrowAction;
        }

        private IAction? GetBestSeedAction(Game game)
        {
            if (game.MyPlayer.Trees[TreeSize.Seed].Any())
            {
                return null;
            }

            var seedActions = game.PossibleActions.OfType<SeedAction>().ToList();
            var bestSoilQuality = SoilQuality.Unusable;
            var bestSeedAction = seedActions.FirstOrDefault();
            foreach (var seedAction in seedActions)
            {
                var cellSoilQuality = game.Board[seedAction.SeedIndex].SoilQuality;
                if (bestSoilQuality < cellSoilQuality)
                {
                    bestSoilQuality = cellSoilQuality;
                    bestSeedAction = seedAction;
                }
            }

            return bestSeedAction;
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

        public const bool ShowInput = true;
    }
}

namespace SpringChallenge2021.Models
{
    public class Cell
    {
        private readonly int[] _neighbours;

        public int Index { get; }

        public Dictionary<HexDirection, Cell?> Neighbours { get; set; }

        public SoilQuality SoilQuality { get; }

        public Cell(int index, SoilQuality soilQuality, int[] neighbours)
        {
            Index = index;
            SoilQuality = soilQuality;
            _neighbours = neighbours;
        }

        public void UpdateNeighbours(IReadOnlyDictionary<int, Cell> board)
        {
            Neighbours = new Dictionary<HexDirection, Cell?>
            {
                {0, _neighbours[0] == -1 ? null : board[_neighbours[0]]},
                {(HexDirection) 1, _neighbours[1] == -1 ? null : board[_neighbours[1]]},
                {(HexDirection) 2, _neighbours[2] == -1 ? null : board[_neighbours[2]]},
                {(HexDirection) 3, _neighbours[3] == -1 ? null : board[_neighbours[3]]},
                {(HexDirection) 4, _neighbours[4] == -1 ? null : board[_neighbours[4]]},
                {(HexDirection) 5, _neighbours[5] == -1 ? null : board[_neighbours[5]]},
            };
        }
    }
}
namespace SpringChallenge2021.Models
{
    public enum HexDirection
    {
        East = 0,
        NorthEast = 1,
        NorthWest = 2,
        West = 3,
        SouthWest = 4,
        SouthEast = 5
    }
}

namespace SpringChallenge2021.Models
{
    public class Player
    {
        private int _sunPoints;
        private int _score;
        private bool _isWaiting;

        public Dictionary<TreeSize, List<Tree>> Trees { get; }

        public Player()
        {
            Trees = new Dictionary<TreeSize, List<Tree>>
            {
                {TreeSize.Seed, new List<Tree>()},
                {TreeSize.Small, new List<Tree>()},
                {TreeSize.Medium, new List<Tree>()},
                {TreeSize.Large, new List<Tree>()},
            };
        }

        public void ReInit()
        {
            foreach (var (_, treeList) in Trees)
            {
                treeList.Clear();
            }
        }

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

        public void AddTree(Tree tree)
        {
            Trees[tree.Size].Add(tree);
        }
    }
}
namespace SpringChallenge2021.Models
{
    public enum SoilQuality
    {
        Unusable = 0,
        Low = 1,
        Medium = 2,
        High = 3
    }
}
namespace SpringChallenge2021.Models
{
    public class Tree
    {
        private bool _isMine;
        private bool _isDormant;

        public int CellIndex { get; }
        public TreeSize Size { get; }

        public Tree(int cellIndex, TreeSize size, bool isMine, bool isDormant)
        {
            CellIndex = cellIndex;
            Size = size;
            _isMine = isMine;
            _isDormant = isDormant;
        }
    }
}
namespace SpringChallenge2021.Models
{
    public enum TreeSize
    {
        Seed = 0,
        Small = 1,
        Medium = 2,
        Large = 3
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