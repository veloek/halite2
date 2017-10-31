using System.Collections.Generic;

namespace Halite2.hlt
{
    public class Player
    {
        private Dictionary<int, Ship> _ships;
        private int _id;

        public Dictionary<int, Ship> Ships => _ships;
        public int Id => _id;

        public Player(int id, Dictionary<int, Ship> ships)
        {
            this._id = id;
            this._ships = ships;
        }
    }
}