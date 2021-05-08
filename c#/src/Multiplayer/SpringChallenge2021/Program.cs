using SpringChallenge2021.Agents;
using SpringChallenge2021.Common.Services;

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
