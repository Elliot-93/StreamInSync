namespace StreamInSync.Models
{
    using System;
    using System.Collections.Generic;

    public class Room
    {
        public Room()
        {
            Members = new List<RoomMember>();
        }

        public int RoomId { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string InviteCode { get; set; }

        public User Owner { get; set; }

        public ICollection<RoomMember> Members { get; set; }

        public string ProgrammeName { get; set; }

        public TimeSpan Runtime { get; set; }

        public DateTime ProgrammeStartTime { get; set; }

        public ICollection<User> UsersFavorited { get; set; }
    }
}