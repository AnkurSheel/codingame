using JoinThePac.Models;

namespace JoinThePac.Agents
{
    public class ReactAgent
    {
        private readonly Map _map;

        private readonly Player _myPlayer;

        public ReactAgent(Map map, Player myPlayer)
        {
            _map = map;
            _myPlayer = myPlayer;
        }

        public string GetAction()
        {
            var pac = _myPlayer.Pac;
            var cell = _map.Cells[pac.Y, pac.X];
            foreach (var (_, neighbour) in cell.Neighbours)
            {
                if (neighbour.HasPellet)
                {
                    return $"MOVE {pac.Id} {neighbour.X} {neighbour.Y}";
                }
            }
            return $"MOVE {pac.Id} {_map.Width/2} {_map.Height/2}";
        }
    }
}
