using System;
using System.Collections.Generic;

namespace JoinThePac.Models
{
    public class Player
    {
        public Player()
        {
            Pacs = new Dictionary<int, Pac>();
        }

        public int Score { get; set; }

        public Dictionary<int, Pac> Pacs { get; }

        public void UpdatePac(int id, int x, int y)
        {
            if (!Pacs.ContainsKey(id))
            {
                Pacs[id] = new Pac(id, x, y);
            }
            else
            {
                Pacs[id].Update(x, y);
            }
        }

        public void Reset()
        {
            foreach (var (_, pac) in Pacs)
            {
                pac.Reset();
            }
        }

        public void ResetVisibleCells(Map map)
        {
            foreach (var (_, pac) in Pacs)
            {
                var pacCell = map.Cells[pac.Position.Y, pac.Position.X];
                foreach (var visibleCell in pacCell.VisibleCells)
                {
                    visibleCell.PelletValue = 0;
                }
            }
        }
    }
}
