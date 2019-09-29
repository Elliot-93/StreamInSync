using StreamInSync.Models;
using System;
using System.Collections.Generic;

namespace StreamInSync.Repository.Interfaces
{
    interface IRoomRepository
    {
        Room Create(CreateRoomVM newRoom, User user);
        Room Get(string username, string password);
        Room Get(int roomId);
        IEnumerable<Room> GetUsersRooms(int userId);
        void AddUser(int roomId, int userId, string connectionId, DateTime lastUpdated);
        int? RemoveUser(int userId, string connectionId);
        int? DisconnectUser(int userId, string connectionId);
        bool UpdateRoomMember(RoomMemberUpdate roomMemberUpdate);
    }
}
