using FallChallenge2022.Common.Services;

namespace FallChallenge2022.Models
{
    public class Tile
    {
        public int ScrapAmount { get; set; }

        public int Owner { get; set; }

        public int NumberOfUnits { get; set; }

        public Position Position { get; }

        private int _recycler;
        private int _canBuild;
        private int _canSpawn;
        private int _inRangeOfRecycler;

        public Tile(int posX, int posY)
        {
            Position = new Position(posX, posY);
            
            var inputs = Io.ReadLine().Split(' ');
            ScrapAmount = int.Parse(inputs[0]);
            Owner = int.Parse(inputs[1]); // 1 = me, 0 = foe, -1 = neutral
            NumberOfUnits = int.Parse(inputs[2]);
            _recycler = int.Parse(inputs[3]);
            _canBuild = int.Parse(inputs[4]);
            _canSpawn = int.Parse(inputs[5]);
            _inRangeOfRecycler = int.Parse(inputs[6]);
        }
    }
}
