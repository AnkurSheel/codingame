using System.Collections.Generic;
using SpringChallenge2021.Common.Services;

namespace SpringChallenge2021.Models
{
    public class Player
    {
        private int _sunPoints;


        private bool _isWaiting;

        public Dictionary<TreeSize, List<Tree>> Trees { get; }
        public int Score { get; private set; }

        public Player()
        {
            Trees = new Dictionary<TreeSize, List<Tree>>
            {
                {TreeSize.Seed, new List<Tree>()},
                {TreeSize.Small, new List<Tree>()},
                {TreeSize.Medium, new List<Tree>()},
                {TreeSize.Large, new List<Tree>()},
            };
        }

        public void ReInit()
        {
            foreach (var (_, treeList) in Trees)
            {
                treeList.Clear();
            }
        }

        public void Parse()
        {
            Io.Debug("Reading Player State");
            var inputs = Io.ReadLine().Split(' ');

            _sunPoints = int.Parse(inputs[0]);
            Score = int.Parse(inputs[1]); // your current score
            _isWaiting = false;

            if (inputs.Length == 3)
            {
                _isWaiting = inputs[2] != "0"; // whether your opponent is asleep until the next day
            }
        }

        public void AddTree(Tree tree)
        {
            Trees[tree.Size].Add(tree);
        }
    }
}
