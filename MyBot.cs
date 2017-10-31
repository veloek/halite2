using Halite2.hlt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Halite2
{
    public class MyBot
    {
        private static List<Move> _moveList = new List<Move>();
        private static GameMap _gameMap;
        private static bool _allPlanetsAreOwned;
        private static Dictionary<int, Mission> _missions =
            new Dictionary<int, Mission>();
        private static Player _player;

        public static void Main(string[] args)
        {
            string name = args.Length > 0 ? args[0] : "T-800";

            _gameMap = Networking.Initialize(name);

            for (;;)
            {
                _moveList.Clear();
                _gameMap.UpdateMap(Networking.ReadLineIntoMetadata());
                _allPlanetsAreOwned = _gameMap.Planets.Values.All(p => p.IsOwned());
                _player = _gameMap.GetMyPlayer();

                var ships = _player.Ships.Values;
                var currentMissions = ships.Where(s => _missions.ContainsKey(s.Id)).ToList();
                var obsoleteMissions = _missions.Keys.Where(k => !_player.Ships.Keys.Contains(k)).ToList();
                foreach (var k in obsoleteMissions)
                    _missions.Remove(k);

                foreach (var ship in currentMissions)
                {
                    try
                    {
                        UpdateMission(ship);
                    }
                    catch (Exception e)
                    {
                        DebugLog.AddLog($"UpdateMission: {e.Message}");
                    }
                }

                foreach (var ship in ships.Except(currentMissions))
                {
                    try
                    {
                        AssignMission(ship);
                    }
                    catch (Exception e)
                    {
                        DebugLog.AddLog($"AssignMission: {e.Message}");
                    }
                }

                Networking.SendMoves(_moveList);
            }
        }

        private static void AssignMission(Ship ship)
        {
            if (_allPlanetsAreOwned)
            {
                var closestEnemy = ship.ClosestEnemy(_gameMap);
                if (closestEnemy != null)
                {
                    _missions[ship.Id] = new Mission
                    {
                        Type = Mission.MissionType.Attack,
                        TargetId = closestEnemy.Id
                    };
                    var newThrustMove = Navigation.NavigateShipToDock(
                        _gameMap, ship, closestEnemy, Constants.MAX_SPEED);
                    if (newThrustMove != null)
                    {
                        _moveList.Add(newThrustMove);
                    }
                }
            }
            else
            {
                var bestPlanet = _gameMap.Planets.Values
                    .Where(p => !p.IsOwned() || (p.Owner == _gameMap.PlayerId && !p.IsFull()))
                    .OrderByDescending(p => p.NumAvailableSpots()/p.DistanceTo(ship))
                    .First();
                if (ship.CanDock(bestPlanet))
                {
                    _missions[ship.Id] = new Mission
                    {
                        Type = Mission.MissionType.StayDocked,
                        TargetId = bestPlanet.Id
                    };
                    _moveList.Add(new DockMove(ship, bestPlanet));
                }
                else
                {
                    _missions[ship.Id] = new Mission
                    {
                        Type = Mission.MissionType.TakePlanet,
                        TargetId = bestPlanet.Id
                    };
                    var newThrustMove = Navigation.NavigateShipToDock(
                        _gameMap, ship, bestPlanet, 7);
                    if (newThrustMove != null)
                    {
                        _moveList.Add(newThrustMove);
                    }
                }
            }
        }

        private static void UpdateMission(Ship ship)
        {
            var targetPlanet = _gameMap.Planets.Values.Where(p => p.Id == _missions[ship.Id].TargetId).FirstOrDefault();
            var targetShip = _gameMap.Ships.Where(s => s.Id == _missions[ship.Id].TargetId).FirstOrDefault();
            switch (_missions[ship.Id].Type)
            {
                case Mission.MissionType.TakePlanet:
                case Mission.MissionType.FillPlanet:
                if (targetPlanet == null)
                {
                    AssignMission(ship);
                    return;
                }

                if (ship.CanDock(targetPlanet))
                {
                    _missions[ship.Id] = new Mission
                    {
                        Type = Mission.MissionType.StayDocked,
                        TargetId = targetPlanet.Id
                    };
                    _moveList.Add(new DockMove(ship, targetPlanet));
                }
                else if (!targetPlanet.IsOwned() || targetPlanet.HasRoom(_gameMap.PlayerId))
                {
                    var newThrustMove = Navigation.NavigateShipToDock(
                        _gameMap, ship, targetPlanet, 7);
                    if (newThrustMove != null)
                    {
                        _moveList.Add(newThrustMove);
                    }
                }
                else
                {
                    AssignMission(ship);
                }
                break;

                case Mission.MissionType.Attack:
                if (targetShip != null)
                {
                    var newThrustMove = Navigation.NavigateShipToDock(
                        _gameMap, ship, targetShip, Constants.MAX_SPEED);
                    if (newThrustMove != null)
                    {
                        _moveList.Add(newThrustMove);
                    }
                }
                else
                {
                    AssignMission(ship);
                }
                break;

                case Mission.MissionType.Suicide:
                break;

                case Mission.MissionType.StayDocked:
                if (targetPlanet == null || targetPlanet.Owner != _gameMap.PlayerId)
                    AssignMission(ship);
                break;
            }
        }
    }
}
