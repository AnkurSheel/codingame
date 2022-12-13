using System.Collections.Generic;

namespace FallChallenge2022
{
    public class Player
    {
        private int _matter;

        public IReadOnlyList<Unit> Units { get; private set; }

        public void ReInit(int matter, IReadOnlyList<Unit> units)
        {
            _matter = matter;
            Units = units;
        }
    }
}
