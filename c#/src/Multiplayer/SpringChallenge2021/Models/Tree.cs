namespace SpringChallenge2021.Models
{
    public class Tree
    {
        private bool _isDormant;

        public int CellIndex { get; }

        public TreeSize Size { get; }

        public bool IsMine { get; }

        public Tree(int cellIndex, TreeSize size, bool isMine, bool isDormant)
        {
            CellIndex = cellIndex;
            Size = size;
            IsMine = isMine;
            _isDormant = isDormant;
        }
    }
}
