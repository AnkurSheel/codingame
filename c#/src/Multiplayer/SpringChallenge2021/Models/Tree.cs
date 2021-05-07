namespace SpringChallenge2021.Models
{
    internal class Tree
    {
        private int _cellIndex;
        private int _size;
        private bool _isMine;
        private bool _isDormant;

        public Tree(int cellIndex, int size, bool isMine, bool isDormant)
        {
            _cellIndex = cellIndex;
            _size = size;
            _isMine = isMine;
            _isDormant = isDormant;
        }
    }
}
