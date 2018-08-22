namespace StreamInSync.Migrations.Site
{
    using StreamInSync.Data;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SiteDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
            MigrationsDirectory = @"Migrations\Site";
        }

        protected override void Seed(SiteDbContext context)
        {
            context.Users.AddOrUpdate(u => u.UserId, StubData.Users().ToArray());
            context.Rooms.AddOrUpdate(r => r.RoomId, StubData.Rooms(context).ToArray());

            context.SaveChanges();            
        }
    }
}
