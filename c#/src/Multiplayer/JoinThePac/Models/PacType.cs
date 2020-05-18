namespace JoinThePac.Models
{
    public enum PacType
    {
        Unknown,

        Rock,

        Paper,

        Scissors
    }

    public static class PacTypeExtensions
    {
        public static PacType FromString(string type)
        {
            if (type == "ROCK")
            {
                return PacType.Rock;
            }

            if (type == "PAPER")
            {
                return PacType.Paper;
            }

            if (type == "SCISSORS")
            {
                return PacType.Scissors;
            }

            return PacType.Unknown;
        }
    }
}
