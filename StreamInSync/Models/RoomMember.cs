namespace StreamInSync.Models
{
    using StreamInSync.Enums;

    public class RoomMember
    {
        public int UserId { get; set; }

        public int RoomId { get; set; }

        public string Username { get; set; }

        public RoomRole Role { get; set; }

        public string ConnectionId { get; set; }


        public User User { get; set; }

        public Room Room { get; set; }
    }
}