namespace SpringChallenge2021.Models
{
    internal class Cell
    {
        private int _index;
        private SoilQuality _soilQuality;
        private int[] _neighbours;

        public Cell(int index, SoilQuality soilQuality, int[] neighbours)
        {
            _index = index;
            _soilQuality = soilQuality;
            _neighbours = neighbours;
        }
    }
}
