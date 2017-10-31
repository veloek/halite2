using System;

namespace Halite2.hlt
{
    public class Position
    {
        private double _xPos;
        private double _yPos;

        public double XPos => _xPos;
        public double YPos => _yPos;

        public Position(double xPos, double yPos) {
            this._xPos = xPos;
            this._yPos = yPos;
        }

        public double DistanceTo(Position target) {
            double dx = _xPos - target.XPos;
            double dy = _yPos - target.YPos;
            return Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        }
        
        public virtual double GetRadius() {
            return 0;
        }

        public int OrientTowardsInDeg(Position target) {
            return Util.AngleRadToDegClipped(OrientTowardsInRad(target));
        }

        public double OrientTowardsInRad(Position target) {
            double dx = target.XPos - _xPos;
            double dy = target.YPos - _yPos;

            return Math.Atan2(dy, dx) + 2 * Math.PI;
        }

        public Position GetClosestPoint(Position target) {
            double radius = target.GetRadius() + Constants.MIN_DISTANCE_FOR_CLOSEST_POINT;
            double angleRad = target.OrientTowardsInRad(this);

            double x = target.XPos + radius * Math.Cos(angleRad);
            double y = target.YPos + radius * Math.Sin(angleRad);

            return new Position(x, y);
        }

        public override bool Equals(Object o) {
            if (this == o) 
                return true;            

            if (o == null || GetType() != o.GetType())
                return false;
            
            Position position = (Position)o;

            if (position == null)
                return false;

            return Equals(position._xPos, _xPos) && Equals(position._yPos, _yPos);
        }

        public override int GetHashCode() {
            return base.GetHashCode();
        }

        public override string ToString() {
            return "Position(" + _xPos + ", " + _yPos + ")";
        }
    }
}
