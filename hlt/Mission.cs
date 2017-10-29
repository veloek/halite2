namespace Halite2.hlt
{
    public class Mission
    {
        public MissionType MissionType { get; set; }
        public Entity Target { get; set; }
    }

    public enum MissionType
    {
        TakePlanet,
        Dock
    }
}