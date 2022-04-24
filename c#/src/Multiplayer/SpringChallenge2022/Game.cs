using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using SpringChallenge2022;
using SpringChallenge2022.Common.Services;
using SpringChallenge2022.Models;

internal class Game
{
    private int _heroesPerPlayer;

    public Player MyPlayer { get; private set; }

    public Player OpponentPlayer { get; private set; }

    public Dictionary<int, Monster> Monsters { get; private set; }

    public void Initialize()
    {
        var inputs = Io.ReadLine().Split(' ');

        // base_x,base_y: The corner of the map representing your base
        var baseX = int.Parse(inputs[0]);
        var baseY = int.Parse(inputs[1]);

        MyPlayer = new Player(new Vector2(baseX, baseY));
        OpponentPlayer = new Player(new Vector2(Constants.BottomRightMap.X - baseX, Constants.BottomRightMap.Y - baseY));

        // heroesPerPlayer: Always 3
        _heroesPerPlayer = int.Parse(Io.ReadLine());
    }

    public void Reinit()
    {
        var inputs = Io.ReadLine().Split(' ');
        var myHealth = int.Parse(inputs[0]); // Your base health
        var myMana = int.Parse(inputs[1]); // Ignore in the first league; Spend ten mana to cast a spell

        inputs = Io.ReadLine().Split(' ');
        var oppHealth = int.Parse(inputs[0]);
        var oppMana = int.Parse(inputs[1]);

        ReInitEntities();

        MyPlayer.Update(myHealth, myMana, Monsters.Values.ToList());
        OpponentPlayer.Update(oppHealth, oppMana, Monsters.Values.ToList());
    }

    private void ReInitEntities()
    {
        var entityCount = int.Parse(Io.ReadLine()); // Amount of heros and monsters you can see

        Monsters = new Dictionary<int, Monster>();

        float heroAngleForStartingPosition = 15;

        for (var i = 0; i < entityCount; i++)
        {
            var inputs = Io.ReadLine().Split(' ');
            var id = int.Parse(inputs[0]); // Unique identifier
            var type = int.Parse(inputs[1]); // 0=monster, 1=your hero, 2=opponent hero
            var x = int.Parse(inputs[2]); // Position of this entity
            var y = int.Parse(inputs[3]);
            var shieldLife = int.Parse(inputs[4]); // Ignore for this league; Count down until shield spell fades
            var isControlled = int.Parse(inputs[5]); // Ignore for this league; Equals 1 when this entity is under a control spell
            var health = int.Parse(inputs[6]); // Remaining health of this monster
            var vx = int.Parse(inputs[7]); // Trajectory of this monster
            var vy = int.Parse(inputs[8]);
            var nearBase = int.Parse(inputs[9]); // 0=monster with no target yet, 1=monster targeting a base
            var threatFor = int.Parse(inputs[10]); // Given this monster's trajectory, is it a threat to 1=your base, 2=your opponent's base, 0=neither

            switch (type)
            {
                case 0:
                    var monster = new Monster(
                        id,
                        new Vector2(x, y),
                        health,
                        new Vector2(vx, vy),
                        nearBase == 1,
                        threatFor);
                    Monsters.Add(id, monster);
                    break;
                case 1:
                    if (MyPlayer.Heroes.ContainsKey(id))
                    {
                        MyPlayer.Heroes[id].Update(new Vector2(x, y));
                    }
                    else
                    {
                        var startingPosition = GetStartingPositionForHero(x, heroAngleForStartingPosition);
                        heroAngleForStartingPosition += 30;

                        MyPlayer.Heroes.Add(id, new Hero(id, new Vector2(x, y), startingPosition));
                    }

                    break;
                case 2:
                    if (OpponentPlayer.Heroes.ContainsKey(id))
                    {
                        OpponentPlayer.Heroes[id].Update(new Vector2(x, y));
                    }
                    else
                    {
                        var position = new Vector2(x, y);
                        OpponentPlayer.Heroes.Add(id, new Hero(id, position, position));
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private Vector2 GetStartingPositionForHero(int posX, float heroAngleForStartingPosition)
    {
        var angleForStartingPosition = posX > Constants.BottomRightMap.X / 2
            ? 270 - heroAngleForStartingPosition
            : heroAngleForStartingPosition;

        var direction = Vector2Extensions.GetDirection(angleForStartingPosition);

        Io.Debug($"Angle: {angleForStartingPosition}, Direction {direction} : Angle {direction.GetAngle()}");

        var startingPosition = MyPlayer.BasePosition + direction * Constants.DistanceFromBaseForStartingPosition;

        Io.Debug($"StartingPosition: {startingPosition}");
        return startingPosition;
    }
}
