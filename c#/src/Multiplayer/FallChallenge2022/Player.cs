using System.Collections.Generic;

namespace FallChallenge2022
{
    internal class Player
    {
        private int _matter;
        private IReadOnlyList<Unit> _units;

        public void ReInit(int matter, IReadOnlyList<Unit> units)
        {
            _matter = matter;
            _units = units;
        }
    }
}
