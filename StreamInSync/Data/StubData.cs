using StreamInSync.Models;
using System;
using System.Collections.Generic;

namespace StreamInSync.Data
{
    public class StubData
    {
        public static IEnumerable<User> Users()
        {
            return new[]
            {
                new User
                {
                    UserId = 1,
                    Username = "UserOne",
                    Password = "password"
                },
                new User
                {
                    UserId = 2,
                    Username = "UserTwo",
                    Password = "passwordTwo"
                }
            };
        }

        public static IEnumerable<Room> Rooms(SiteDbContext dbContext)
        {
            return new[]
            {
                new Room
                {
                    RoomId = 1,
                    Name = "Show of the week",
                    InviteCode = "4gg4y7ur",
                    Owner = dbContext.Users.Find(1),
                    Members = new List<RoomMember>(),
                    ProgrammeName = "The Handmaids Tale",
                    Runtime = new TimeSpan(0, 40, 0),
                    ProgrammeStartTime = DateTime.Now.AddHours(2)
                },
                new Room
                {
                    RoomId = 2,
                    Name = "Show of the month",
                    InviteCode = "4gg4y7ur",
                    Owner = dbContext.Users.Find(1),
                    Members = new List<RoomMember>(),
                    ProgrammeName = "Mr Robot",
                    Runtime = new TimeSpan(0, 40, 0),
                    ProgrammeStartTime = DateTime.Now.AddDays(2)
                }
            };
        }
    }
}