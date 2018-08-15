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
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        RoomId = c.Int(nullable: false),
                        Username = c.String(),
                        Role = c.Int(nullable: false),
                        ConnectionId = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rooms", t => t.RoomId, cascadeDelete: true)
                .Index(t => t.RoomId);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Password = c.String(),
                        InviteCode = c.String(),
                        ProgrammeName = c.String(),
                        Runtime = c.Time(nullable: false, precision: 7),
                        ProgrammeStartTime = c.DateTime(nullable: false),
                        Owner_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.Owner_Id)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(maxLength: 30, unicode: false),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Username, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rooms", "Owner_Id", "dbo.Users");
            DropForeignKey("dbo.RoomMembers", "RoomId", "dbo.Rooms");
            DropIndex("dbo.Users", new[] { "Username" });
            DropIndex("dbo.Rooms", new[] { "Owner_Id" });
            DropIndex("dbo.RoomMembers", new[] { "RoomId" });
            DropTable("dbo.Users");
            DropTable("dbo.Rooms");
            DropTable("dbo.RoomMembers");
        }
    }
}
