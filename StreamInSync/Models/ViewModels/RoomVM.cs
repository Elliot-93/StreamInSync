namespace StreamInSync.Models
{
    public class RoomVM
    {
        public RoomVM(Room room, string jsonRoom)
        {
            Room = room;
            JsonRoom = jsonRoom;
        }

        public Room Room { get; private set; }

        public string JsonRoom { get; private set; }
    }
}