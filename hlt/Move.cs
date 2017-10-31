namespace Halite2.hlt
{
    public class Move
    {
        public enum MoveType { Noop, Thrust, Dock, Undock }

        private MoveType _type;
        private Ship _ship;

        public MoveType Type => _type;
        public Ship Ship => _ship;

        public Move(MoveType type, Ship ship)
        {
            this._type = type;
            this._ship = ship;
        }
    }
}
