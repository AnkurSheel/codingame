using System.Collections.Generic;
using FallChallenge2022.Action;
using FallChallenge2022.Common.Services;
using FallChallenge2022.Models;
using SpringChallenge2021.Actions;

namespace FallChallenge2022.Agent
{
    public class SimpleAgentV1 : IAgent
    {
        public IReadOnlyList<IAction> GetActions(Game game)
        {
            var actions = new List<IAction>();
            Io.Debug(game.Width.ToString());
            Io.Debug(game.Height.ToString());

            var alreadyTargetedPositions = new HashSet<Position>();

            foreach (var unit in game.MyPlayer.Units)
            {
                var action = GetMoveActionForUnit(game, unit, alreadyTargetedPositions);

                if (action != null)
                {
                    actions.Add(action);
                }
            }

            if (actions.Count == 0)
            {
                actions.Add(new WaitAction());
            }

            return actions;
        }

        private IAction GetMoveActionForUnit(Game game, Unit unit, HashSet<Position> alreadyTargetedPositions)
        {
            var newPosition = TryGetValidPosition(game, new Position(unit.Tile.Position.X - 1, unit.Tile.Position.Y), alreadyTargetedPositions);

            if (newPosition != null)
            {
                alreadyTargetedPositions.Add(newPosition);
                return new MoveAction(unit.Tile.Position, newPosition);
            }

            newPosition = TryGetValidPosition(game, new Position(unit.Tile.Position.X + 1, unit.Tile.Position.Y), alreadyTargetedPositions);

            if (newPosition != null)
            {
                alreadyTargetedPositions.Add(newPosition);
                return new MoveAction(unit.Tile.Position, newPosition);
            }

            newPosition = TryGetValidPosition(game, new Position(unit.Tile.Position.X, unit.Tile.Position.Y - 1), alreadyTargetedPositions);

            if (newPosition != null)
            {
                alreadyTargetedPositions.Add(newPosition);
                return new MoveAction(unit.Tile.Position, newPosition);
            }

            newPosition = TryGetValidPosition(game, new Position(unit.Tile.Position.X, unit.Tile.Position.Y + 1), alreadyTargetedPositions);

            if (newPosition != null)
            {
                alreadyTargetedPositions.Add(newPosition);
                return new MoveAction(unit.Tile.Position, newPosition);
            }

            return new MoveAction(unit.Tile.Position, new Position(game.Width / 2, game.Height / 2));
        }

        private Position? TryGetValidPosition(Game game, Position to, HashSet<Position> alreadyTargetedTiles)
        {
            var newTile = game.GetTileAt(to);

            return newTile != null && newTile.Owner != 1 && newTile.ScrapAmount > 0 && !alreadyTargetedTiles.Contains(newTile.Position)
                ? newTile.Position
                : null;
        }
    }
}
