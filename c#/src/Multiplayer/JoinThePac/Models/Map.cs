﻿using System.Collections.Generic;
using System.Text;

namespace JoinThePac.Models
{
    public class Map
    {
        public int Width { get; }

        public int Height { get; }

        public Cell[,] Cells { get; }

        public HashSet<Cell> SuperPellets { get; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Cell[height, width];
            SuperPellets = new HashSet<Cell>();
        }

        public void Build(int row, string line)
        {
            for (var i = 0; i < line.Length; i++)
            {
                var ch = line[i];
                var cellType = CellType.Floor;
                if (ch == '#')
                {
                    cellType = CellType.Wall;
                }

                Cells[row, i] = new Cell(i, row, cellType);
            }
        }

        public void SetupCells()
        {
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    CheckAndAddNeighbour(Cells[i, j], j, i - 1, Direction.North);
                    CheckAndAddNeighbour(Cells[i, j], j - 1, i, Direction.East);
                    CheckAndAddNeighbour(Cells[i, j], j + 1, i, Direction.West);
                    CheckAndAddNeighbour(Cells[i, j], j, i + 1, Direction.South);
                }
            }

            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    AddVisibleCells(Cells[i, j], Direction.North);
                    AddVisibleCells(Cells[i, j], Direction.South);
                    AddVisibleCells(Cells[i, j], Direction.East);
                    AddVisibleCells(Cells[i, j], Direction.West);
                }
            }
        }

        private void AddVisibleCells(Cell cell, Direction direction)
        {
            var currentCell = cell;
            var count = 0;
            while (true)
            {
                if ((count > 0 && currentCell.Equals(cell)) || !currentCell.Neighbours.ContainsKey(direction))
                {
                    break;
                }
                else
                {
                    count++;
                    var neighbour = currentCell.Neighbours[direction];
                    cell.VisibleCells.Add(neighbour);
                    currentCell = neighbour;
                }
            }
        }

        private void CheckAndAddNeighbour(Cell cell, int x, int y, Direction direction)
        {
            if (x < 0)
            {
                x = Width - 1;
            }
            else if (x >= Width)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = Height - 1;
            }
            else if (y >= Height)
            {
                y = 0;
            }

            var cellType = Cells[y, x].Type;
            if (cellType == CellType.Floor)
            {
                cell.Neighbours.Add(direction, Cells[y, x]);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            //sb.Append("  |");
            //for (var i = 0; i < Width; i++)
            //{
            //    sb.Append($"{i:D2} ");
            //}

            //sb.AppendLine();
            //for (var i = 0; i < Width; i++)
            //{
            //    sb.Append($"---");
            //}
            //sb.AppendLine();
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    //if (j == 0)
                    //{
                    //    sb.Append($"{i:D2}|");
                    //}
                    if (Cells[i, j].Type == CellType.Floor)
                    {
                        //sb.Append($"{Cells[i,j].Neighbours.Count:D2} ");
                        sb.Append($"{Cells[i, j].Neighbours.Count}");
                    }
                    else if (Cells[i, j].Type == CellType.Wall)
                    {
                        //sb.Append("## ");
                        sb.Append("#");
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public void SetCellValue(int x, int y, int pelletValue)
        {
            var cell = Cells[y, x];
            cell.PelletValue = pelletValue;
            if (pelletValue == 10)
            {
                SuperPellets.Add(cell);
            }
        }

        public void ResetSuperPellets()
        {
            SuperPellets.Clear();
        }
    }
}
