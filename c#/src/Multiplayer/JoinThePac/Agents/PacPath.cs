using System.Collections.Generic;
using System.Diagnostics;

using JoinThePac.Models;

namespace JoinThePac.Agents
{
    [DebuggerDisplay("pac={Pac.Id} cell={Cell.Position} path={Path.Count}")]
    public class PacPath
    {
        public Pac Pac { get; }

        public Cell Cell { get; }

        public List<Cell> Path { get; }

        public PacPath(Pac pac, Cell cell, List<Cell> path)
        {
            Pac = pac;
            Cell = cell;
            Path = path;
        }
    }
}
