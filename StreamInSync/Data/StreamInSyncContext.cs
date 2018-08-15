namespace StreamInSync.Data
{
    using StreamInSync.Models;
    using System.Data.Entity;

    public class SiteDbContext : DbContext
    {
        public SiteDbContext() : base("StreamInSync")
        {           
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<RoomMember> RoomMembers { get; set; }
    }
}