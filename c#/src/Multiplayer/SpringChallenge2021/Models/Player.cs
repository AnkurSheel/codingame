using SpringChallenge2021.Common.Services;

namespace SpringChallenge2021.Models
{
    public class Player
    {
        private int _sunPoints;
        private int _score;
        private bool _isWaiting;

        public void Parse()
        {
            Io.Debug("Reading Player State");
            var inputs = Io.ReadLine().Split(' ');

            _sunPoints = int.Parse(inputs[0]);
            _score = int.Parse(inputs[1]); // your current score
            _isWaiting = false;

            if (inputs.Length == 3)
            {
                _isWaiting = inputs[2] != "0"; // whether your opponent is asleep until the next day
            }
        }
    }
}
