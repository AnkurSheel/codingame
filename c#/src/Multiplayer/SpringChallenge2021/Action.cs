namespace SpringChallenge2021
{
    internal class Action
    {
        public const string WAIT = "WAIT";
        public const string SEED = "SEED";
        public const string GROW = "GROW";
        public const string COMPLETE = "COMPLETE";

        public static Action Parse(string action)
        {
            var parts = action.Split(" ");
            switch (parts[0])
            {
                case WAIT:
                    return new Action(WAIT);
                case SEED:
                    return new Action(SEED, int.Parse(parts[1]), int.Parse(parts[2]));
                case GROW:
                case COMPLETE:
                default:
                    return new Action(parts[0], int.Parse(parts[1]));
            }
        }

        private readonly string _type;
        private readonly int _targetCellIdx;
        private readonly int _sourceCellIdx;

        public Action(string type, int sourceCellIdx, int targetCellIdx)
        {
            _type = type;
            _targetCellIdx = targetCellIdx;
            _sourceCellIdx = sourceCellIdx;
        }

        public Action(string type, int targetCellIdx)
            : this(type, 0, targetCellIdx)
        {
        }

        public Action(string type)
            : this(type, 0, 0)
        {
        }

        public override string ToString()
        {
            if (_type == WAIT)
            {
                return WAIT;
            }

            if (_type == SEED)
            {
                return $"{SEED} {_sourceCellIdx} {_targetCellIdx}";
            }

            return $"{_type} {_targetCellIdx}";
        }
    }
}
