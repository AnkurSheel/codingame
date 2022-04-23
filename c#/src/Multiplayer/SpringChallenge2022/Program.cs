using SpringChallenge2022.Agents;
using SpringChallenge2022.Common.Services;

internal class Player
{
    private static void Main(string[] args)
    {
        Io.Initialize();

        var game = new Game();
        var agent = new AgentWood2Boss();

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
