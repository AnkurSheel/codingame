using System.Collections.Generic;

using FallChallenge2020.Agents;
using FallChallenge2020.Common;
using FallChallenge2020.Models;

namespace FallChallenge2020
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Io.Initialize();
            var agent = new SimpleAgent();
            // game loop
            while (true)
            {
                var game = new Game ();
                game.Initialize();
                var action = agent.GetAction(game);

                Io.WriteLine(action.GetAction());
            }
        }

        
    }
}
