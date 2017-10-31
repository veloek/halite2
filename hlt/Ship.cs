using System.Linq;

namespace Halite2.hlt
{
    public enum DockingStatus { Undocked = 0, Docking = 1, Docked = 2, Undocking = 3 }

    public class Ship : Entity
    {
        private DockingStatus _dockingStatus;
        private int _dockedPlanet;
        private int _dockingProgress;
        private int _weaponCooldown;

        public Mission Mission { get; set; }
        public DockingStatus DockingStatus => _dockingStatus;
        public int DockedPlanet => _dockedPlanet;
        public int DockingProgress => _dockingProgress;
        public int WeaponCooldown => _weaponCooldown;

        public Ship(int owner, int id, double xPos, double yPos,
                    int health, DockingStatus dockingStatus, int dockedPlanet,
                    int dockingProgress, int weaponCooldown)
            : base(owner, id, xPos, yPos, health, Constants.SHIP_RADIUS)
        {
            this._dockingStatus = dockingStatus;
            this._dockedPlanet = dockedPlanet;
            this._dockingProgress = dockingProgress;
            this._weaponCooldown = weaponCooldown;
        }

        public bool CanDock(Planet planet)
        {
            return DistanceTo(planet) <= Constants.DOCK_RADIUS + planet.GetRadius();
        }

        public Planet ClosestAvailablePlanet(GameMap gameMap)
        {
            return gameMap.Planets.Values
                .Where(p => !p.IsOwned() || (p.Owner == gameMap.PlayerId && !p.IsFull()))
                .OrderBy(p => p.DistanceTo(this))
                .FirstOrDefault();
        }

        public Ship ClosestEnemy(GameMap gameMap)
        {
            return gameMap.Ships
                .Where(s => s.Owner != gameMap.PlayerId)
                .OrderBy(e => e.DistanceTo(this))
                .FirstOrDefault();
        }

        public override string ToString()
        {
            return "Ship[" +
                    base.ToString() +
                    ", dockingStatus=" + _dockingStatus +
                    ", dockedPlanet=" + _dockedPlanet +
                    ", dockingProgress=" + _dockingProgress +
                    ", weaponCooldown=" + _weaponCooldown +
                    "]";
        }
    }
}
