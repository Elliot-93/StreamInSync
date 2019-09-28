namespace StreamInSync.Models
{
    public class RoomMemberUpdate
    {
        public RoomMemberUpdate(int userId, ProgrammeTimeUpdate programmeTimeUpdate)
        {
            UserId = userId;
            ProgrammeTimeUpdate = programmeTimeUpdate;
        }

        public int UserId { get; }

        public ProgrammeTimeUpdate ProgrammeTimeUpdate { get; }
    }
}