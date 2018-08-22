namespace StreamInSync.Models
{
    public class UserRoomVM
    {
        public UserRoomVM(Room room, string link)
        {
            Room = room;
            Link = link;
        }

        public Room Room { get; private set; }

        public string Link { get; private set; }
    }
}