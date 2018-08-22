namespace StreamInSync.Migrations.Site
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoomMembers",
                c => new
                    {
                        RoomId = c.Int(nullable: false),
                        UserId = c.Int(nullable: false),
                        Username = c.String(),
                        Role = c.Int(nullable: false),
                        ConnectionId = c.String(),
                    })
                .PrimaryKey(t => new { t.RoomId, t.UserId })
                .ForeignKey("dbo.Rooms", t => t.RoomId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.RoomId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        RoomId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Password = c.String(),
                        InviteCode = c.String(),
                        ProgrammeName = c.String(),
                        Runtime = c.Time(nullable: false, precision: 7),
                        ProgrammeStartTime = c.DateTime(nullable: false),
                        Owner_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.RoomId)
                .ForeignKey("dbo.Users", t => t.Owner_UserId)
                .Index(t => t.Owner_UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 30, unicode: false),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.UserId)
                .Index(t => t.Username, unique: true);
            
            CreateTable(
                "dbo.UserFavoriteRooms",
                c => new
                    {
                        UserId = c.Int(nullable: false),
                        RoomId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoomId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.Rooms", t => t.RoomId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoomId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rooms", "Owner_UserId", "dbo.Users");
            DropForeignKey("dbo.RoomMembers", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserFavoriteRooms", "RoomId", "dbo.Rooms");
            DropForeignKey("dbo.UserFavoriteRooms", "UserId", "dbo.Users");
            DropForeignKey("dbo.RoomMembers", "RoomId", "dbo.Rooms");
            DropIndex("dbo.UserFavoriteRooms", new[] { "RoomId" });
            DropIndex("dbo.UserFavoriteRooms", new[] { "UserId" });
            DropIndex("dbo.Users", new[] { "Username" });
            DropIndex("dbo.Rooms", new[] { "Owner_UserId" });
            DropIndex("dbo.RoomMembers", new[] { "UserId" });
            DropIndex("dbo.RoomMembers", new[] { "RoomId" });
            DropTable("dbo.UserFavoriteRooms");
            DropTable("dbo.Users");
            DropTable("dbo.Rooms");
            DropTable("dbo.RoomMembers");
        }
    }
}
