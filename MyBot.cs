using Halite2.hlt;
using System.Collections.Generic;
using System.Linq;

namespace Halite2
{
    public class MyBot
    {
        private static List<Move> moveList = new List<Move>();
        private static Dictionary<Planet, int> shipsHeadingTowardsPlanet =
            new Dictionary<Planet, int>();

        public static void Main(string[] args)
        {
            string name = args.Length > 0 ? args[0] : "T-800";

            Networking networking = new Networking();
            GameMap gameMap = networking.Initialize(name);

            for (;;)
            {
                moveList.Clear();
                shipsHeadingTowardsPlanet.Clear();
                gameMap.UpdateMap(Networking.ReadLineIntoMetadata());

                foreach (Ship ship in gameMap.GetMyPlayer().GetShips().Values)
                {
                    if (ship.GetDockingStatus() != Ship.DockingStatus.Undocked)
                    {
                        continue;
                    }

                    var allIsOwned = gameMap.GetAllPlanets().All(p => p.Value.IsOwned());
                    var closestAvailablePlanet = gameMap.GetAllPlanets().Values
                        .Where(p => !p.IsOwned() || p.HasRoom(gameMap.GetMyPlayerId()))
                        .Where(p => ShouldDispatchMoreShips(p))
                        .OrderBy(p => p.GetDistanceTo(ship))
                        .FirstOrDefault();
                    var closestEnemy = gameMap.GetAllShips()
                        .Where(s => s.GetOwner() != gameMap.GetMyPlayerId())
                        .OrderBy(e => e.GetDistanceTo(ship))
                        .FirstOrDefault();

                    if (allIsOwned)
                    {
                        if (closestEnemy != null)
                        {
                            var newThrustMove = Navigation.NavigateShipToDock(
                                gameMap, ship, closestEnemy, Constants.MAX_SPEED);
                            if (newThrustMove != null)
                            {
                                moveList.Add(newThrustMove);
                            }
                        }
                    }
                    else if (ship.CanDock(closestAvailablePlanet))
                    {
                        moveList.Add(new DockMove(ship, closestAvailablePlanet));
                    }
                    else
                    {
                        var newThrustMove = Navigation.NavigateShipToDock(
                            gameMap, ship, closestAvailablePlanet, 7);
                        if (newThrustMove != null)
                        {
                            moveList.Add(newThrustMove);
                        }
                    }
                }
                Networking.SendMoves(moveList);
            }
        }

        private static bool ShouldDispatchMoreShips(Planet p)
        {
            var numShips = 0;
            return (shipsHeadingTowardsPlanet.TryGetValue(p, out numShips) && numShips < p.NumAvailableSpots())
                || numShips == 0;
        }
    }
}
