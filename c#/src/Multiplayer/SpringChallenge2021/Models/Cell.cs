namespace SpringChallenge2021.Models
{
    public class Cell
    {
        private int _index;
        private int[] _neighbours;

        public SoilQuality SoilQuality { get; }

        public Cell(int index, SoilQuality soilQuality, int[] neighbours)
        {
            _index = index;
            SoilQuality = soilQuality;
            _neighbours = neighbours;
        }
    }
}