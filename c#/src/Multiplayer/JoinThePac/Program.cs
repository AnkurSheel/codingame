using JoinThePac.Agents;
using JoinThePac.Models;
using JoinThePac.Services;

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

            var agent = new ReactAgent(map, myPlayer);

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
                Io.WriteLine(agent.GetAction()); // MOVE <pacId> <x> <y>
            }
        }
    }
}
