using StreamInSync.Models;
using System;
using System.Collections.Generic;

namespace StreamInSync.Services
{
    public interface IRoomService
    {
        Room Create(CreateRoomVM newRoom, User user);
        Room Get(string inviteCode, string password);
        Room Get(int roomId);
        IEnumerable<Room> GetAllPublicRooms();
        IEnumerable<Room> GetUsersRooms(int userId);
        void AddUser(int roomId, int userId, string connectionId, DateTime lastUpdated);
        int? RemoveUser(int userId, string connectionId);
        int? DisconnectUser(int userId, string connectionId);
        bool UpdateRoomMember(RoomMemberUpdate roomMemberUpdate);
    }
}
