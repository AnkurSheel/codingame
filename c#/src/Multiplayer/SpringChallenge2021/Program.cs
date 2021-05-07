using SpringChallenge2021.Common.Services;

namespace SpringChallenge2021
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Io.Initialize();

            var game = new Game();


            // game loop
            while (true)
            {
                game.ReadGameState();

                var action = game.GetNextAction();
                Io.WriteLine(action.GetOutputAction());
            }
        }
    }
}
