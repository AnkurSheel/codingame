using SpringChallenge2021.Actions;

namespace SpringChallenge2021
{
    internal class Action
    {
        private const string WAIT = "WAIT";
        private const string SEED = "SEED";
        private const string GROW = "GROW";
        private const string COMPLETE = "COMPLETE";

        public static IAction Parse(string action)
        {
            var parts = action.Split(" ");
            switch (parts[0])
            {
                case GROW:
                    return new GrowAction(int.Parse(parts[1]));
                case COMPLETE:
                    return new CompleteAction(int.Parse(parts[1]));
                case WAIT:
                case SEED:
                default:
                    return new WaitAction();
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
