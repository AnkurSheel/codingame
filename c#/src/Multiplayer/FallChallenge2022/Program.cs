using FallChallenge2022.Agent;
using FallChallenge2022.Common.Services;

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

                var action = agent.GetAction(game);
                Io.WriteLine(action.GetOutputAction());
            }
        }
    }
}
