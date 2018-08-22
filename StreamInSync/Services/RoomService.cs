using StreamInSync.Models;
using StreamInSync.Respository;
using StreamInSync.Respository.Interfaces;
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

        public void AddUser(int roomId, int userId, string connectionId)
        {
            roomRepository.AddUser(roomId, userId, connectionId);
        }

        public int? RemoveUser(int userId, string connectionId)
        {
            return roomRepository.RemoveUser(userId, connectionId);
        }
    }
}
