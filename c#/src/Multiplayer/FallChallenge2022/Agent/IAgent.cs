using System.Collections.Generic;
using SpringChallenge2021.Actions;

namespace FallChallenge2022.Agent
{
    public interface IAgent
    {
        IReadOnlyList<IAction> GetActions(Game game);
    }
}
