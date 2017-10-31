namespace Halite2.hlt
{
    public class Entity : Position
    {
        private int _owner;
        private int _id;
        private int _health;
        private double _radius;

        public int Owner => _owner;
        public int Id => _id;
        public int Health => _health;

        public Entity(int owner, int id, double xPos, double yPos, int health, double radius)
            : base(xPos, yPos)
        {
            this._owner = owner;
            this._id = id;
            this._health = health;
            this._radius = radius;
        }

        public override double GetRadius()
        {
            return _radius;
        }

        public override string ToString()
        {
            return "Entity[" +
                    base.ToString() +
                    ", owner=" + _owner +
                    ", id=" + _id +
                    ", health=" + _health +
                    ", radius=" + _radius +
                    "]";
        }
    }
}
