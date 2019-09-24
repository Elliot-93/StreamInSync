namespace StreamInSync.Models
{
    using StreamInSync.Enums;
    using System;

    public class RoomMember
    {
        public int UserId { get; set; }

        public int RoomId { get; set; }

        public string Username { get; set; }

        public RoomRole Role { get; set; }

        public string ConnectionId { get; set; }


        public TimeSpan ProgrammeTime { get; set; }

        public DateTime LastUpdated { get; set; }

        public bool InBreak { get; set; }

        public TimeSpan? BreakTime { get; set; }


        public User User { get; set; }

        public Room Room { get; set; }
    }
}