using SpringChallenge2022.Agents;
using SpringChallenge2022.Common.Services;

internal class Program
{
    private static void Main(string[] args)
    {
        Io.Initialize();

        var game = new Game();
        var agent = new SilverBoss();

        game.Initialize();

        // game loop
        while (true)
        {
            game.Reinit();
            var actions = agent.GetAction(game);

            foreach (var action in actions)
            {
                Io.WriteLine(action.GetOutputAction());
            }
        }
    }
}
