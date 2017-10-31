using System.Collections.Generic;

namespace Halite2.hlt
{
    public class Planet: Entity
    {
        private int _remainingProduction;
        private int _currentProduction;
        private int _dockingSpots;
        private IList<int> _dockedShips;

        public int RemainingProduction => _remainingProduction;
        public int CurrentProduction => _currentProduction;
        public int DockingSpots => _dockingSpots;
        public IList<int> DockedShips => _dockedShips;

        public Planet(int owner, int id, double xPos, double yPos, int health,
                      double radius, int dockingSpots, int currentProduction,
                      int remainingProduction, List<int> dockedShips)
        :base(owner, id, xPos, yPos, health, radius)
        {
            this._dockingSpots = dockingSpots;
            this._currentProduction = currentProduction;
            this._remainingProduction = remainingProduction;
            this._dockedShips = dockedShips.AsReadOnly();
        }

        public int NumAvailableSpots()
        {
            return _dockingSpots - _dockedShips.Count;
        }

        public bool IsFull()
        {
            return NumAvailableSpots() == 0;
        }

        public bool IsOwned()
        {
            return Owner != -1;
        }

        public bool HasRoom(int playerId)
        {
            return Owner == playerId && !IsFull();
        }
        
        public override string ToString()
        {
            return "Planet[" +
                    base.ToString() +
                    ", remainingProduction=" + _remainingProduction +
                    ", currentProduction=" + _currentProduction +
                    ", dockingSpots=" + _dockingSpots +
                    ", dockedShips=" + _dockedShips +
                    "]";
        }
    }
}