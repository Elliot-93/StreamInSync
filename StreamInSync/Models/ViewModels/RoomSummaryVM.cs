namespace StreamInSync.Models
{
    public class RoomSummaryVM
    {
        public RoomSummaryVM(Room room, string link)
        {
            Room = room;
            Link = link;
        }

        public Room Room { get; private set; }

        public string Link { get; private set; }
    }
}