using System.Collections.Generic;

namespace JoinThePac.Models
{
    public class Player
    {
        public Player()
        {
            Pacs = new List<Pac>();
        }

        public int Score { get; set; }

        public List<Pac> Pacs { get; }
    }
}
