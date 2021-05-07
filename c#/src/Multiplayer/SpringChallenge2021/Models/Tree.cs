namespace SpringChallenge2021.Models
{
    public class Tree
    {
        private int _cellIndex;
        private TreeSize _size;
        private bool _isMine;
        private bool _isDormant;

        public Tree(int cellIndex, TreeSize size, bool isMine, bool isDormant)
        {
            _cellIndex = cellIndex;
            _size = size;
            _isMine = isMine;
            _isDormant = isDormant;
        }
    }
}
