namespace StreamInSync.Models
{
    public class RoomSummaryVM
    {
        public RoomSummaryVM(Room room, string joinRoomLink)
        {
            Room = room;
            JoinRoomLink = joinRoomLink;
        }

        public Room Room { get; private set; }

        public string JoinRoomLink { get; private set; }
    }
}