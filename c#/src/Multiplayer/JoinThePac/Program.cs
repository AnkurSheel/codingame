using JoinThePac.Agents;
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
            Io.Initialize();

            var game = new Game();

            game.InitializeMap();

            var agent = new ReactAgent(game);

            // game loop
            while (true)
            {
                game.ReadTurn();

                Io.WriteLine(agent.Think()); // MOVE <pacId> <x> <y>
            }
        }
    }
}
