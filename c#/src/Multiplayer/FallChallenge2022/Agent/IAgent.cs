using SpringChallenge2021.Actions;

namespace FallChallenge2022.Agent
{
    public interface IAgent
    {
        IAction GetAction(Game game);
    }
}
