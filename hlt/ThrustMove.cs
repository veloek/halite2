namespace Halite2.hlt
{
    public class ThrustMove : Move
    {
        private int _angleDeg;
        private int _thrust;

        public int Angle => _angleDeg;
        public int Thrust => _thrust;

        public ThrustMove(Ship ship, int angleDeg, int thrust)
            : base(MoveType.Thrust, ship)
        {
            this._thrust = thrust;
            this._angleDeg = angleDeg;
        }
    }
}
