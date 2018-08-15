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

        public int Id { get; set; }

        public string Name { get; set; }

        public string Password { get; set; }

        public string InviteCode { get; set; }

        public virtual User Owner { get; set; }

        public virtual IList<RoomMember> Members { get; set; }

        public string ProgrammeName { get; set; }

        public TimeSpan Runtime { get; set; }

        public DateTime ProgrammeStartTime { get; set; }
    }
}