using System;


 // 07/05/2021 06:40


internal class Program
{
    private static void Main(string[] args)
    {
        string[] inputs;
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
        }

        // game loop
        while (true)
        {
            var day = int.Parse(Console.ReadLine()); // the game lasts 24 days: 0-23
            var nutrients = int.Parse(Console.ReadLine()); // the base score you gain from the next COMPLETE action
            inputs = Console.ReadLine().Split(' ');
            var sun = int.Parse(inputs[0]); // your sun points
            var score = int.Parse(inputs[1]); // your current score
            inputs = Console.ReadLine().Split(' ');
            var oppSun = int.Parse(inputs[0]); // opponent's sun points
            var oppScore = int.Parse(inputs[1]); // opponent's score
            var oppIsWaiting = inputs[2] != "0"; // whether your opponent is asleep until the next day
            var numberOfTrees = int.Parse(Console.ReadLine()); // the current amount of trees
            for (var i = 0; i < numberOfTrees; i++)
            {
                inputs = Console.ReadLine().Split(' ');
                var cellIndex = int.Parse(inputs[0]); // location of this tree
                var size = int.Parse(inputs[1]); // size of this tree: 0-3
                var isMine = inputs[2] != "0"; // 1 if this is your tree
                var isDormant = inputs[3] != "0"; // 1 if this tree is dormant
            }

            var numberOfPossibleActions = int.Parse(Console.ReadLine()); // all legal actions
            for (var i = 0; i < numberOfPossibleActions; i++)
            {
                var possibleAction = Console.ReadLine(); // try printing something from here to start with
            }

            // Write an action using Console.WriteLine()
            // To debug: Console.Error.WriteLine("Debug messages...");


            // GROW cellIdx | SEED sourceIdx targetIdx | COMPLETE cellIdx | WAIT <message>
            Console.WriteLine("WAIT");
        }
    }
}