namespace Halite2.hlt
{
    public class Ship : Entity
    {
        public enum DockingStatus { Undocked = 0, Docking = 1, Docked = 2, Undocking = 3 }

        private DockingStatus dockingStatus;
        private int dockedPlanet;
        private int dockingProgress;
        private int weaponCooldown;
        public Mission Mission { get; set; };

        public Ship(int owner, int id, double xPos, double yPos,
                    int health, DockingStatus dockingStatus, int dockedPlanet,
                    int dockingProgress, int weaponCooldown)
            : base(owner, id, xPos, yPos, health, Constants.SHIP_RADIUS)
        {
            this.dockingStatus = dockingStatus;
            this.dockedPlanet = dockedPlanet;
            this.dockingProgress = dockingProgress;
            this.weaponCooldown = weaponCooldown;
        }

        public int WeaponCooldown
        {
            get { return weaponCooldown; }
        }

        public DockingStatus DockingStatus
        {
            get { return dockingStatus; }
        }

        public int DockingProgress
        {
            get { return dockingProgress; }
        }

        public int DockedPlanet()
        {
            get { return dockedPlanet; }
        }

        public bool CanDock(Planet planet)
        {
            return GetDistanceTo(planet) <= Constants.DOCK_RADIUS + planet.GetRadius();
        }

        public override string ToString()
        {
            return "Ship[" +
                    base.ToString() +
                    ", dockingStatus=" + dockingStatus +
                    ", dockedPlanet=" + dockedPlanet +
                    ", dockingProgress=" + dockingProgress +
                    ", weaponCooldown=" + weaponCooldown +
                    "]";
        }
    }
}
