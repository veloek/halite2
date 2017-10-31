namespace Halite2.hlt
{
    public class DockMove : Move
    {
        private long _destinationId;

        public long DestinationId
        {
            get { return _destinationId; }
        }

        public DockMove(Ship ship, Planet planet)
            : base(MoveType.Dock, ship)
        {
            _destinationId = planet.Id;
        }
    }
}