using SpringChallenge2021.Actions;

namespace FallChallenge2022.Agent
{
    public class SimpleAgentV1 : IAgent
    {
        public IAction GetAction(Game game)
        {
            return new WaitAction();
        }
    }
}
