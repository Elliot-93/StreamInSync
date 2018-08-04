using StreamInSync.Models;

namespace StreamInSync.Respository.Interfaces
{
    interface IRoomRepository
    {
        Room Create(CreateRoomVM newRoom, User user);
        Room Get(string username, string password);
        Room Get(int roomId);
        void AddUser(int roomId, int userId, string connectionId);
        int? RemoveUser(int userId, string connectionId);
    }
}
