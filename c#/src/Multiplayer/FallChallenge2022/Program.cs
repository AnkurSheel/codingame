using System.Linq;
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

                var actions = agent.GetActions(game);
                var output = string.Join(";", actions.Select(x => x.GetOutputAction()));
                Io.WriteLine(output);
            }
        }
    }
}
