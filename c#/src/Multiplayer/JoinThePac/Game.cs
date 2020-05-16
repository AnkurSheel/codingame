using System.Text;

using JoinThePac.Models;
using JoinThePac.Services;

namespace JoinThePac
{
    public class Game
    {
        public Game()
        {
            MyPlayer = new Player();
            OpponentPlayer = new Player();
        }

        public Map Map { get; private set; }

        public Player MyPlayer { get; }

        public Player OpponentPlayer { get; }

        public void InitializeMap()
        {
            var inputs = Io.ReadLine().Split(' ');
            var width = int.Parse(inputs[0]); // size of the grid
            var height = int.Parse(inputs[1]); // top left corner is (x=0, y=0)

            Map = new Map(width, height);

            for (var i = 0; i < height; i++)
            {
                var row = Io.ReadLine(); // one line of the grid: space " " is floor, pound "#" is wall
                Map.Build(i, row);
            }

            Map.SetupCells();
            //Io.Debug($"Built Map{Environment.NewLine}{map}");
        }

        public void ReadTurn()
        {
            Reset();
            var inputs = Io.ReadLine().Split(' ');
            MyPlayer.Score = int.Parse(inputs[0]);

            OpponentPlayer.Score = int.Parse(inputs[1]);
            var visiblePacCount = int.Parse(Io.ReadLine()); // all your pacs and enemy pacs in sight

            for (var i = 0; i < visiblePacCount; i++)
            {
                inputs = Io.ReadLine().Split(' ');
                var pacId = int.Parse(inputs[0]); // pac number (unique within a team)
                var mine = inputs[1] != "0"; // true if this pac is yours
                var x = int.Parse(inputs[2]); // position in the grid
                var y = int.Parse(inputs[3]); // position in the grid
                var typeId = inputs[4]; // unused in wood leagues
                var speedTurnsLeft = int.Parse(inputs[5]); // unused in wood leagues
                var abilityCooldown = int.Parse(inputs[6]); // unused in wood leagues
                Map.SetCellValue(x,y,0);
                if (mine)
                {
                    MyPlayer.UpdatePac(pacId, x, y, PacTypeExtensions.FromString(typeId), speedTurnsLeft, abilityCooldown);
                }
                else
                {
                    OpponentPlayer.UpdatePac(pacId, x, y, PacTypeExtensions.FromString(typeId), speedTurnsLeft, abilityCooldown);
                }
            }

            Map.ResetSuperPellets();
            MyPlayer.ResetVisibleCells(Map);

            var visiblePelletCount = int.Parse(Io.ReadLine()); // all pellets in sight
            for (var i = 0; i < visiblePelletCount; i++)
            {
                inputs = Io.ReadLine().Split(' ');
                var x = int.Parse(inputs[0]);
                var y = int.Parse(inputs[1]);
                var value = int.Parse(inputs[2]); // amount of points this pellet is worth
                Map.SetCellValue(x, y, value);
            }

            //DebugPelletValues();
        }

        private void Reset()
        {
            MyPlayer.Reset();
            OpponentPlayer.Reset();
        }

        private void DebugPelletValues()
        {
            var sb = new StringBuilder();
            for (var i = 0; i < Map.Height; i++)
            {
                for (var j = 0; j < Map.Width; j++)
                {
                    if (Map.Cells[i, j].Type == CellType.Floor)
                    {
                        var pelletValue = Map.Cells[i, j].PelletValue;
                        var a = 'U';
                        if (pelletValue == 10)
                        {
                            a = 'S';
                        }
                        else if (pelletValue == 0)
                        {
                            a = 'E';
                        }
                        else if (pelletValue == 1)
                        {
                            a = 'P';
                        }

                        sb.Append($"{a}");
                    }
                    else if (Map.Cells[i, j].Type == CellType.Wall)
                    {
                        sb.Append("#");
                    }
                }

                sb.AppendLine();
            }

            Io.Debug(sb.ToString());
        }
    }
}
