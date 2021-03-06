﻿using System;
using System.Collections.Generic;
using System.Linq;

using JoinThePac.Models;

namespace JoinThePac.Services
{
    public static class BFS
    {
        public static List<Cell> GetClosestCells(Cell from, Func<Cell, bool> condition, Func<Cell, bool> obstacleCondition, int maxLength)
        {
            var open = new List<Cell> { from };
            var seen = new HashSet<Cell> { from };
            var cells = new List<Cell>();

            while (open.Any() && cells.Count < maxLength)
            {
                var currentCell = open.First();
                open.RemoveAt(0);

                if (condition(currentCell))
                {
                    cells.Add(currentCell);
                }

                foreach (var (_, neighbour) in currentCell.Neighbours)
                {
                    if (seen.Add(neighbour) && !obstacleCondition(neighbour))
                    {
                        open.Add(neighbour);
                    }
                }
            }

            return cells;
        }

        public static List<Cell> GetPath(Cell from, Cell to, Func<Cell, bool> obstacleCondition)
        {
            if (obstacleCondition(to))
            {
                return null;
            }

            var fromCell = new BfsCell(null, from);
            var open = new List<BfsCell> { fromCell };
            var seen = new HashSet<Cell> { from };
            while (open.Any())
            {
                var currentCell = open.First();
                open.RemoveAt(0);

                if (currentCell.Cell.Equals(to))
                {
                    var list = new List<Cell>();
                    while (!currentCell.Cell.Equals(from))
                    {
                        list.Insert(0, currentCell.Cell);
                        currentCell = currentCell.Parent;
                    }

                    return list;
                }

                foreach (var (_, neighbour) in currentCell.Cell.Neighbours)
                {
                    if (seen.Add(neighbour) && !obstacleCondition(neighbour))
                    {
                        open.Add(new BfsCell(currentCell, neighbour));
                    }
                }
            }

            return null;
        }
    }

    public class BfsCell
    {
        public BfsCell(BfsCell parent, Cell cell)
        {
            Parent = parent;
            Cell = cell;
        }

        public BfsCell Parent { get; }

        public Cell Cell { get; }
    }
}
