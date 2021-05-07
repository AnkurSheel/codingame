using SpringChallenge2021.Agents;
using SpringChallenge2021.Common.Services;

namespace SpringChallenge2021
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Io.Initialize();

            var game = new Game();
            var agent = new SimpleAgent();

            // game loop
            while (true)
            {
                game.ReadGameState();

                var action = agent.GetAction(game);
                Io.WriteLine(action.GetOutputAction());
            }
        }
    }
}
