namespace StreamInSync.Data
{
    using StreamInSync.Models;
    using System.Data.Entity;

    public class SiteDbContext : DbContext
    {
        public SiteDbContext() : base("StreamInSync")
        {
            //Configuration.LazyLoadingEnabled = false;
            //Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Room> Rooms { get; set; }

        public DbSet<RoomMember> RoomMembers { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // todo: remove this when finished
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<SiteDbContext>()); 

            modelBuilder.Entity<RoomMember>()
                .HasKey(r => new { r.RoomId, r.UserId });

            modelBuilder.Entity<User>()
                .HasMany(s => s.FavoriteRooms)
                .WithMany(c => c.UsersFavorited)
                .Map(cs =>
                {
                    cs.MapLeftKey("UserId");
                    cs.MapRightKey("RoomId");
                    cs.ToTable("UserFavoriteRooms");
                });
        }
    }
}