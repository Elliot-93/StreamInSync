namespace StreamInSync.Models
{
    using StreamInSync.Enums;
    using System;

    public class ProgrammeTimeUpdate
    {
        public int RoomId { get; set; }

        public int ProgrammeTimeSecs { get; set; }

        public DateTime LastUpdated { get; set; }

        public PlayStatus PlayStatus { get; set; }

        public bool InBreak { get; set; }

        public int? BreakTimeSecs { get; set; }
    }
}