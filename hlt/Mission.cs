namespace Halite2.hlt
{
    public class Mission
    {
       public enum MissionType
        {
            TakePlanet,
            FillPlanet,
            Attack,
            Suicide,
            StayDocked
        }

        public MissionType Type { get; set; }
        public int TargetId { get; set; }
    }
}