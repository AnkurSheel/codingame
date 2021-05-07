using System.Collections.Generic;
using System.Linq;
using SpringChallenge2021.Common.Services;
using SpringChallenge2021.Models;

namespace SpringChallenge2021
{
    internal class Game
    {
        private int _day;
        private int _nutrients;
        private readonly List<Cell> _board;
        private readonly List<Action> _possibleActions;
        private readonly List<Tree> trees;
        private int _mySun;
        private int _opponentSun;
        private int _myScore;
        private int _opponentScore;
        private bool _opponentIsWaiting;

        public Game()
        {
            _board = new List<Cell>();
            _possibleActions = new List<Action>();
            trees = new List<Tree>();

            GenerateBoard();
        }

        public void ReadGameState()
        {
            _day = int.Parse(Io.ReadLine()); // the game lasts 24 days: 0-23
            _nutrients = int.Parse(Io.ReadLine()); // the base score you gain from the next COMPLETE action
            var inputs = Io.ReadLine().Split(' ');
            _mySun = int.Parse(inputs[0]); // your sun points
            _myScore = int.Parse(inputs[1]); // your current score
            inputs = Io.ReadLine().Split(' ');
            _opponentSun = int.Parse(inputs[0]); // opponent's sun points
            _opponentScore = int.Parse(inputs[1]); // opponent's score
            _opponentIsWaiting = inputs[2] != "0"; // whether your opponent is asleep until the next day

            trees.Clear();
            var numberOfTrees = int.Parse(Io.ReadLine()); // the current amount of trees
            for (var i = 0; i < numberOfTrees; i++)
            {
                inputs = Io.ReadLine().Split(' ');
                var cellIndex = int.Parse(inputs[0]); // location of this tree
                var size = int.Parse(inputs[1]); // size of this tree: 0-3
                var isMine = inputs[2] != "0"; // 1 if this is your tree
                var isDormant = inputs[3] != "0"; // 1 if this tree is dormant
                var tree = new Tree(cellIndex, size, isMine, isDormant);
                trees.Add(tree);
            }
        }

        public Action GetNextAction()
        {
            _possibleActions.Clear();
            var numberOfPossibleMoves = int.Parse(Io.ReadLine());
            for (var i = 0; i < numberOfPossibleMoves; i++)
            {
                var possibleMove = Io.ReadLine();
                _possibleActions.Add(Action.Parse(possibleMove));
            }

            return _possibleActions.First();
        }

        private void GenerateBoard()
        {
            string[] inputs;
            var numberOfCells = int.Parse(Io.ReadLine()); // 37
            for (var i = 0; i < numberOfCells; i++)
            {
                inputs = Io.ReadLine().Split(' ');
                var index = int.Parse(inputs[0]); // 0 is the center cell, the next cells spiral outwards
                var richness = int.Parse(inputs[1]); // 0 if the cell is unusable, 1-3 for usable cells
                var neigh0 = int.Parse(inputs[2]); // the index of the neighbouring cell for each direction
                var neigh1 = int.Parse(inputs[3]);
                var neigh2 = int.Parse(inputs[4]);
                var neigh3 = int.Parse(inputs[5]);
                var neigh4 = int.Parse(inputs[6]);
                var neigh5 = int.Parse(inputs[7]);
                int[] neighs = {neigh0, neigh1, neigh2, neigh3, neigh4, neigh5};
                var cell = new Cell(index, richness, neighs);
                _board.Add(cell);
            }
        }
    }
}
