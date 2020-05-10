using System.Text;

using JoinThePac.Services;

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
            if (x < 0)
            {
                x = Width - 1;
            }

            if (x >= Width - 1)
            {
                x = 0;
            }

            if (y < 0)
            {
                y = Height - 1;
            }

            if (y >= Height - 1)
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

        public void ClearPellets()
        {
            foreach (var cell in Cells)
            {
                cell.Pellet = 0;
            }
        }
    }
}
