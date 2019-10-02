namespace StreamInSync.Repository
{
    using StreamInSync.Data;
    using StreamInSync.Enums;
    using StreamInSync.Models;
    using StreamInSync.Repository.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;

    public class RoomRepository : IRoomRepository
    {
        private readonly SiteDbContext dbContext;

        public RoomRepository()
        {
            dbContext = new SiteDbContext();
        }

        public Room Create(CreateRoomVM newRoom, User user)
        {
            // to do: implement Ioc per web request dbcontext then this might not be needed? https://stackoverflow.com/questions/10585478/one-dbcontext-per-web-request-why
            dbContext.Users.Attach(user);

            var room = new Room
            {
                Name = newRoom.Name,
                InviteCode = GenerateInviteCode(),
                Password = newRoom.Password,
                Owner = user,
                ProgrammeName = newRoom.ProgrammeName,
                Runtime = newRoom.RuntimeInSeconds,
                ProgrammeStartTime = newRoom.ProgrammeStartTime
            };

            dbContext.Rooms.Add(room);
            dbContext.SaveChanges();
            
            return room;
        }

        public Room Get(string inviteCode, string password)
        {
            return dbContext.Rooms.FirstOrDefault(r => 
                r.InviteCode.Equals(inviteCode, StringComparison.Ordinal) 
                && r.InviteCode.Equals(inviteCode, StringComparison.Ordinal));
        }

        public Room Get(int roomId)
        {
            return dbContext.Rooms
                .Where(r => r.RoomId == roomId)
                .Include(r => r.Owner)
                .Include(r => r.Members)
                .FirstOrDefault();
        }

        public IEnumerable<Room> GetAllPublicRooms()
        {
            return dbContext.Rooms
                .Include(r => r.Owner)
                .Where(r => r.Password == null);
        }

        public IEnumerable<Room> GetUsersRooms(int userId)
        {
            return dbContext.Rooms
                .Include(r => r.Owner)
                .Where(r => r.Owner.UserId == userId);
        }

        public void AddUser(int roomId, int userId, string connectionId, DateTime lastUpdated)
        {
            var user = dbContext.Users.Find(userId);

            var roomMembers = dbContext.Rooms.Include(r => r.Members).Single(r => r.RoomId == roomId).Members;

            var exisitingUser = roomMembers.FirstOrDefault(rm => rm.UserId == userId);
            if (exisitingUser != null)
            {
                exisitingUser.ConnectionId = connectionId;
            }
            else
            {
                roomMembers.Add(new RoomMember
                {
                    UserId = userId,
                    Username = user.Username,
                    Role = RoomRole.Watcher,
                    ConnectionId = connectionId, 
                    LastUpdated = lastUpdated,
                    ProgrammeTimeSecs = 0
                });
            }

            dbContext.SaveChanges();
        }

        public int? RemoveUser(int userId, string connectionId)
        {
            var memberToRemove = dbContext.RoomMembers
                .FirstOrDefault(m => m.ConnectionId == connectionId && m.UserId == userId);

            if (memberToRemove == null)
            {
                return null;
            }

            dbContext.RoomMembers.Remove(memberToRemove);
            dbContext.SaveChanges();
            return memberToRemove.RoomId;
        }

        public int? DisconnectUser(int userId, string connectionId)
        {
            var memberToDisconnect = dbContext.RoomMembers
                .FirstOrDefault(m => m.ConnectionId == connectionId && m.UserId == userId);

            if (memberToDisconnect == null)
            {
                return null;
            }

            memberToDisconnect.ConnectionId = null;
            dbContext.SaveChanges();
            return memberToDisconnect.RoomId;
        }

        public bool UpdateRoomMember(RoomMemberUpdate roomMemberUpdate)
        {
            var userProgrammeUpdate = roomMemberUpdate.ProgrammeTimeUpdate;
            var memberToUpdate = dbContext.RoomMembers
                .FirstOrDefault(m => m.RoomId == userProgrammeUpdate.RoomId && m.UserId == roomMemberUpdate.UserId);

            if (memberToUpdate == null)
            {
                return false;
            }

            memberToUpdate.ProgrammeTimeSecs = userProgrammeUpdate.ProgrammeTimeSecs;
            memberToUpdate.LastUpdated = userProgrammeUpdate.LastUpdated;
            memberToUpdate.PlayStatus = userProgrammeUpdate.PlayStatus;
            memberToUpdate.InBreak = userProgrammeUpdate.InBreak;
            memberToUpdate.BreakTimeSecs = userProgrammeUpdate.BreakTimeSecs;

            return dbContext.SaveChanges() > 0;
        }

        // ToDo: Put Limit on and log error
        private string GenerateInviteCode()
        {
            string generatedCode = null;

            do
            {
                generatedCode = Guid.NewGuid().ToString("n").Substring(0, 8);
            }
            while (dbContext.Rooms.Any(r => r.InviteCode == generatedCode));

            return generatedCode;
        }
    }
}