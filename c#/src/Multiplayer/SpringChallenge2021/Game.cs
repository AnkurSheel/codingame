using System.Collections.Generic;
using SpringChallenge2021.Actions;
using SpringChallenge2021.Common.Services;
using SpringChallenge2021.Models;

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

        public HexDirection SunDirection { get; set; }

        public HashSet<Cell> Shadows { get; }

        public HashSet<Cell> ShadowsNextDay { get; set; }

        public Game()
        {
            Board = new Dictionary<int, Cell>();
            PossibleActions = new List<IAction>();
            Trees = new Dictionary<int, Tree>();
            MyPlayer = new Player();
            _opponentPlayer = new Player();

            GenerateBoard();
            Shadows = new HashSet<Cell>();
            ShadowsNextDay = new HashSet<Cell>();
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
            Io.Debug($"Next Sun Direction {sunDirection}");
            GetShadows(sunDirection, ShadowsNextDay);
        }

        private void GetShadows(HexDirection sunDirection, ISet<Cell> shadows)
        {
            foreach (var (treeIndex, tree) in Trees)
            {
                var cell = Board[treeIndex];
                for (var i = 0; i < (int) tree.Size && cell != null; i++)
                {
                    var neighbour = cell.Neighbours[sunDirection];
                    if (neighbour != null)
                    {
                        shadows.Add(neighbour);
                    }

                    cell = neighbour;
                }
            }
        }
    }
}
