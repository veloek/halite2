using Halite2.hlt;
using System.Collections.Generic;
using System.Linq;

namespace Halite2
{
    public class MyBot
    {
        private static List<Move> _moveList = new List<Move>();
        private static GameMap _gameMap;
        private static Dictionary<Planet, int> _shipsHeadingTowardsPlanet =
            new Dictionary<Planet, int>();

        public static void Main(string[] args)
        {
            string name = args.Length > 0 ? args[0] : "T-800";

            _gameMap = Networking.Initialize(name);

            for (;;)
            {
                _moveList.Clear();
                _shipsHeadingTowardsPlanet.Clear();
                _gameMap.UpdateMap(Networking.ReadLineIntoMetadata());

                foreach (var ship in _gameMap.GetMyPlayer().Ships.Values)
                {
                    if (ship.DockingStatus != DockingStatus.Undocked)
                    {
                        continue;
                    }

                    var allIsOwned = _gameMap.Planets.All(p => p.Value.IsOwned());
                    var closestAvailablePlanet = _gameMap.Planets.Values
                        .Where(p => !p.IsOwned() || p.HasRoom(_gameMap.PlayerId))
                        .Where(p => ShouldDispatchMoreShips(p))
                        .OrderBy(p => p.DistanceTo(ship))
                        .FirstOrDefault();
                    var closestEnemy = _gameMap.Ships
                        .Where(s => s.Owner != _gameMap.PlayerId)
                        .OrderBy(e => e.DistanceTo(ship))
                        .FirstOrDefault();

                    if (allIsOwned)
                    {
                        if (closestEnemy != null)
                        {
                            var newThrustMove = Navigation.NavigateShipToDock(
                                _gameMap, ship, closestEnemy, Constants.MAX_SPEED);
                            if (newThrustMove != null)
                            {
                                _moveList.Add(newThrustMove);
                            }
                        }
                    }
                    else if (ship.CanDock(closestAvailablePlanet))
                    {
                        _moveList.Add(new DockMove(ship, closestAvailablePlanet));
                    }
                    else
                    {
                        var newThrustMove = Navigation.NavigateShipToDock(
                            _gameMap, ship, closestAvailablePlanet, 7);
                        if (newThrustMove != null)
                        {
                            _moveList.Add(newThrustMove);
                        }
                    }
                }
                Networking.SendMoves(_moveList);
            }
        }

        private static bool ShouldDispatchMoreShips(Planet p)
        {
            var numShips = 0;
            return (_shipsHeadingTowardsPlanet.TryGetValue(p, out numShips) && numShips < p.NumAvailableSpots())
                || numShips == 0;
        }
    }
}
