using System;
using System.Text;

namespace JoinThePac.Models
{
    public class Map
    {
        public int Width { get; }

        public int Height { get; }

        public Cell[,] Cells { get; }

        public Map(int width, int height)
        {
            Width = width;
            Height = height;
            Cells = new Cell[height, width];
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
        }

        private void CheckAndAddNeighbour(Cell cell, int x, int y, Direction direction)
        {
            if (IsValid(x, y))
            {
                cell.Neighbours.Add(direction, Cells[y, x]);
            }
        }

        private bool IsValid(int x, int y)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
            {
                return false;
            }

            var cellType = Cells[y, x].Type;
            return cellType == CellType.Floor;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Height; i++)
            {
                for (var j = 0; j < Width; j++)
                {
                    if (Cells[i, j].Type == CellType.Floor)
                    {
                        sb.Append($"{Cells[i,j].Neighbours.Count}");
                    }
                    else if (Cells[i, j].Type == CellType.Wall)
                    {
                        sb.Append('#');
                    }
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        public void ClearPellets()
        {
            foreach (var cell in Cells)
            {
                cell.Pellet = 0;
            }
        }
    }
}
