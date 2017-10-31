using System;
using System.Collections.Generic;
using System.Linq;

namespace Halite2.hlt
{
    public class GameMap
    {
        private int _width, _height;
        private int _playerId;
        private List<Player> _players;
        private IList<Player> _playersUnmodifiable;
        private Dictionary<int, Planet> _planets;
        private List<Ship> _allShips;
        private IList<Ship> _allShipsUnmodifiable;

        // used only during parsing to reduce memory allocations
        private List<Ship> _currentShips = new List<Ship>();

        public int Height => _height;
        public int Width => _width;
        public int PlayerId => _playerId;
        public IList<Player> Players => _playersUnmodifiable;
        public Dictionary<int, Planet> Planets => _planets;
        public IList<Ship> Ships => _allShipsUnmodifiable;

        public GameMap(int width, int height, int playerId)
        {
            this._width = width;
            this._height = height;
            this._playerId = playerId;

            _players = new List<Player>(Constants.MAX_PLAYERS);
            _playersUnmodifiable = _players.AsReadOnly();
            _planets = new Dictionary<int, Planet>();
            _allShips = new List<Ship>();
            _allShipsUnmodifiable = _allShips.AsReadOnly();
        }

        public Player GetMyPlayer() => _playersUnmodifiable[_playerId];

        public Ship GetShip(int playerId, int entityId)
        {
            return _players[_playerId].Ships[entityId];
        }

        public List<Entity> ObjectsBetween(Position start, Position target)
        {
            List<Entity> entitiesFound = new List<Entity>();

            AddEntitiesBetween(entitiesFound, start, target, _planets.Values.ToList<Entity>());
            AddEntitiesBetween(entitiesFound, start, target, _allShips.ToList<Entity>());

            return entitiesFound;
        }

        private static void AddEntitiesBetween(List<Entity> entitiesFound,
                                               Position start, Position target,
                                               ICollection<Entity> entitiesToCheck)
        {

            foreach (Entity entity in entitiesToCheck)
            {
                if (entity.Equals(start) || entity.Equals(target))
                {
                    continue;
                }
                if (Collision.segmentCircleIntersect(start, target, entity, Constants.FORECAST_FUDGE_FACTOR))
                {
                    entitiesFound.Add(entity);
                }
            }
        }

        public Dictionary<double, Entity> NearbyEntitiesByDistance(Entity entity)
        {
            Dictionary<double, Entity> entityByDistance = new Dictionary<double, Entity>();

            foreach (Planet planet in _planets.Values)
            {
                if (planet.Equals(entity))
                {
                    continue;
                }
                entityByDistance[entity.DistanceTo(planet)] = planet;
            }

            foreach (Ship ship in _allShips)
            {
                if (ship.Equals(entity))
                {
                    continue;
                }
                entityByDistance[entity.DistanceTo(ship)] = ship;
            }

            return entityByDistance;
        }

        public GameMap UpdateMap(Metadata mapMetadata)
        {
            int numberOfPlayers = MetadataParser.ParsePlayerNum(mapMetadata);

            _players.Clear();
            _planets.Clear();
            _allShips.Clear();

            // update players info
            for (int i = 0; i < numberOfPlayers; ++i)
            {
                _currentShips.Clear();
                Dictionary<int, Ship> currentPlayerShips = new Dictionary<int, Ship>();
                int playerId = MetadataParser.ParsePlayerId(mapMetadata);

                Player currentPlayer = new Player(playerId, currentPlayerShips);
                MetadataParser.PopulateShipList(_currentShips, playerId, mapMetadata);
                _allShips.AddRange(_currentShips);

                foreach (Ship ship in _currentShips)
                {
                    currentPlayerShips[ship.Id] = ship;
                }
                _players.Add(currentPlayer);
            }

            int numberOfPlanets = int.Parse(mapMetadata.Pop());

            for (int i = 0; i < numberOfPlanets; ++i)
            {
                List<int> dockedShips = new List<int>();
                Planet planet = MetadataParser.NewPlanetFromMetadata(dockedShips, mapMetadata);
                _planets[planet.Id] = planet;
            }

            if (!mapMetadata.IsEmpty())
            {
                throw new InvalidOperationException("Failed to parse data from Halite game engine. Please contact maintainers.");
            }

            return this;
        }
    }
}
