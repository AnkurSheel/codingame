using System;
using System.Collections.Generic;
using System.Linq;

using JoinThePac.Models;

namespace JoinThePac.Services
{
    public static class BFS
    {
        public static Cell GetClosestCell(Cell from, Func<Cell, bool> condition)
        {
            var open = new List<Cell> { from };
            var seen = new HashSet<Cell> { from };
            while (open.Any())
            {
                var currentCell = open.First();
                open.RemoveAt(0);

                if (condition(currentCell))
                {
                    return currentCell;
                }

                foreach (var (_, neighbour) in currentCell.Neighbours)
                {
                    if (seen.Add(neighbour))
                    {
                        open.Add(neighbour);
                    }
                }
            }

            return null;
        }
    }
}
