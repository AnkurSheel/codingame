namespace SpringChallenge2021
{
    internal class Cell
    {
        private int _index;
        private int _richness;
        private int[] _neighbours;

        public Cell(int index, int richness, int[] neighbours)
        {
            _index = index;
            _richness = richness;
            _neighbours = neighbours;
        }
    }
}
