using StreamInSync.Models;
using StreamInSync.Repository;
using StreamInSync.Repository.Interfaces;
using System;
using System.Collections.Generic;

namespace StreamInSync.Services
{
    public class RoomService : IRoomService
    {
        private readonly IRoomRepository roomRepository;
        private readonly IUserRepository userRepository;

        public RoomService()
        {
            roomRepository = new RoomRepository();
            userRepository = new UserRepository();
        }

        public Room Create(CreateRoomVM newRoom, User user)
        {
            return roomRepository.Create(newRoom, user);
        }

        public Room Get(string inviteCode, string password)
        {
            return roomRepository.Get(inviteCode, password);
        }

        public Room Get(int roomId)
        {
            return roomRepository.Get(roomId);
        }

        public IEnumerable<Room> GetUsersRooms(int userId)
        {
            return roomRepository.GetUsersRooms(userId);
        }

        public void AddUser(int roomId, int userId, string connectionId, DateTime lastUpdated)
        {
            roomRepository.AddUser(roomId, userId, connectionId, lastUpdated);
        }

        public int? RemoveUser(int userId, string connectionId)
        {
            return roomRepository.RemoveUser(userId, connectionId);
        }

        public bool UpdateRoomMember(RoomMemberUpdate roomMemberUpdate)
        {
            return roomRepository.UpdateRoomMember(roomMemberUpdate);
        }
    }
}
