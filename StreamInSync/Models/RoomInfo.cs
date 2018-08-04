namespace StreamInSync.Models
{
    public class RoomInfo
    {
        public RoomInfo(int id, string name, int userCount, string leadModName)
        {
            Id = id;
            Name = name;
            UserCount = UserCount;
            LeadModName = leadModName;
        }

        public int Id { get; private set; }

        public string Name { get; private set; }

        public string LeadModName { get; private set; }

        public int UserCount { get; private set; }
    }
}