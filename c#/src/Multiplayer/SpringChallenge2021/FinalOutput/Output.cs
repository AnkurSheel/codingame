using System;
using System.Collections.Generic;
using System.Linq;


 // 07/05/2021 06:44


internal class Cell
{
    public int index;
    public int richness;
    public int[] neighbours;

    public Cell(int index, int richness, int[] neighbours)
    {
        this.index = index;
        this.richness = richness;
        this.neighbours = neighbours;
    }
}

internal class Tree
{
    public int cellIndex;
    public int size;
    public bool isMine;
    public bool isDormant;

    public Tree(int cellIndex, int size, bool isMine, bool isDormant)
    {
        this.cellIndex = cellIndex;
        this.size = size;
        this.isMine = isMine;
        this.isDormant = isDormant;
    }
}

internal class Action
{
    public const string WAIT = "WAIT";
    public const string SEED = "SEED";
    public const string GROW = "GROW";
    public const string COMPLETE = "COMPLETE";

    public static Action Parse(string action)
    {
        var parts = action.Split(" ");
        switch (parts[0])
        {
            case WAIT:
                return new Action(WAIT);
            case SEED:
                return new Action(SEED, int.Parse(parts[1]), int.Parse(parts[2]));
            case GROW:
            case COMPLETE:
            default:
                return new Action(parts[0], int.Parse(parts[1]));
        }
    }

    public string type;
    public int targetCellIdx;
    public int sourceCellIdx;

    public Action(string type, int sourceCellIdx, int targetCellIdx)
    {
        this.type = type;
        this.targetCellIdx = targetCellIdx;
        this.sourceCellIdx = sourceCellIdx;
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
        if (type == WAIT)
        {
            return WAIT;
        }

        if (type == SEED)
        {
            return string.Format("{0} {1} {2}", SEED, sourceCellIdx, targetCellIdx);
        }

        return string.Format("{0} {1}", type, targetCellIdx);
    }
}

internal class Game
{
    public int day;
    public int nutrients;
    public List<Cell> board;
    public List<Action> possibleActions;
    public List<Tree> trees;
    public int mySun, opponentSun;
    public int myScore, opponentScore;
    public bool opponentIsWaiting;

    public Game()
    {
        board = new List<Cell>();
        possibleActions = new List<Action>();
        trees = new List<Tree>();
    }

    public Action GetNextAction()
    {
        // TODO: write your algorithm here
        return possibleActions.First();
    }
}

internal class Player
{
    private static void Main(string[] args)
    {
        string[] inputs;

        var game = new Game();

        var numberOfCells = int.Parse(Console.ReadLine()); // 37
        for (var i = 0; i < numberOfCells; i++)
        {
            inputs = Console.ReadLine().Split(' ');
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
            game.board.Add(cell);
        }

        // game loop
        while (true)
        {
            game.day = int.Parse(Console.ReadLine()); // the game lasts 24 days: 0-23
            game.nutrients = int.Parse(Console.ReadLine()); // the base score you gain from the next COMPLETE action
            inputs = Console.ReadLine().Split(' ');
            game.mySun = int.Parse(inputs[0]); // your sun points
            game.myScore = int.Parse(inputs[1]); // your current score
            inputs = Console.ReadLine().Split(' ');
            game.opponentSun = int.Parse(inputs[0]); // opponent's sun points
            game.opponentScore = int.Parse(inputs[1]); // opponent's score
            game.opponentIsWaiting = inputs[2] != "0"; // whether your opponent is asleep until the next day

            game.trees.Clear();
            var numberOfTrees = int.Parse(Console.ReadLine()); // the current amount of trees
            for (var i = 0; i < numberOfTrees; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var cellIndex = int.Parse(inputs[0]); // location of this tree
                var size = int.Parse(inputs[1]); // size of this tree: 0-3
                var isMine = inputs[2] != "0"; // 1 if this is your tree
                var isDormant = inputs[3] != "0"; // 1 if this tree is dormant
                var tree = new Tree(cellIndex, size, isMine, isDormant);
                game.trees.Add(tree);
            }

            game.possibleActions.Clear();
            var numberOfPossibleMoves = int.Parse(Console.ReadLine());
            for (var i = 0; i < numberOfPossibleMoves; i++)
            {
                var possibleMove = Console.ReadLine();
                game.possibleActions.Add(Action.Parse(possibleMove));
            }

            var action = game.GetNextAction();
            Console.WriteLine(action);
        }
    }
}