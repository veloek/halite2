using Halite2.hlt;
using System.Collections.Generic;
using System.Linq;

namespace Halite2
{
    public class MyBot
    {

        public static void Main(string[] args)
        {
            string name = args.Length > 0 ? args[0] : "T-800";

            Networking networking = new Networking();
            GameMap gameMap = networking.Initialize(name);

            List<Move> moveList = new List<Move>();
            for (;;)
            {
                moveList.Clear();
                gameMap.UpdateMap(Networking.ReadLineIntoMetadata());

                foreach (Ship ship in gameMap.GetMyPlayer().GetShips().Values)
                {
                    if (ship.GetDockingStatus() != Ship.DockingStatus.Undocked)
                    {
                        continue;
                    }

                    var allIsOwned = gameMap.GetAllPlanets().All(p => p.Value.IsOwned());
                    var closestAvailablePlanet = gameMap.GetAllPlanets().Values
                        .Where(p => !p.IsOwned() || (p.GetOwner() == gameMap.GetMyPlayerId() && !p.IsFull()))
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
                            ThrustMove newThrustMove = Navigation.NavigateShipToDock(gameMap, ship, closestEnemy, Constants.MAX_SPEED);
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
                        ThrustMove newThrustMove = Navigation.NavigateShipToDock(gameMap, ship, closestAvailablePlanet, Constants.MAX_SPEED / 2);
                        if (newThrustMove != null)
                        {
                            moveList.Add(newThrustMove);
                        }
                    }
                }
                Networking.SendMoves(moveList);
            }
        }
    }
}
