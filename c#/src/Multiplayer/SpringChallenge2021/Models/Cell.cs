using System.Collections.Generic;

namespace SpringChallenge2021.Models
{
    public class Cell
    {
        private readonly int[] _neighbours;

        public int Index { get; }

        public Dictionary<HexDirection, Cell?> Neighbours { get; set; }

        public SoilQuality SoilQuality { get; }

        public Cell(int index, SoilQuality soilQuality, int[] neighbours)
        {
            Index = index;
            SoilQuality = soilQuality;
            _neighbours = neighbours;
        }

        public void UpdateNeighbours(IReadOnlyDictionary<int, Cell> board)
        {
            Neighbours = new Dictionary<HexDirection, Cell?>
            {
                {0, _neighbours[0] == -1 ? null : board[_neighbours[0]]},
                {(HexDirection) 1, _neighbours[1] == -1 ? null : board[_neighbours[1]]},
                {(HexDirection) 2, _neighbours[2] == -1 ? null : board[_neighbours[2]]},
                {(HexDirection) 3, _neighbours[3] == -1 ? null : board[_neighbours[3]]},
                {(HexDirection) 4, _neighbours[4] == -1 ? null : board[_neighbours[4]]},
                {(HexDirection) 5, _neighbours[5] == -1 ? null : board[_neighbours[5]]},
            };
        }
    }
}
