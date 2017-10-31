namespace Halite2.hlt
{
    public enum MissionType
    {
        TakePlanet,
        FillPlanet,
        Attack,
        Suicide,
        StayDocked
    }

    public class Mission
    {
        public MissionType MissionType { get; set; }
        public Entity Target { get; set; }
    }
}