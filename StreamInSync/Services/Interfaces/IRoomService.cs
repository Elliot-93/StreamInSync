using StreamInSync.Models;

namespace StreamInSync.Services
{
    public interface IRoomService
    {
        Room Create(CreateRoomVM newRoom, User user);

        Room Get(string name, string password);

        Room Get(int roomId);

        void AddUser(int roomId, int userId, string connectionId);

        int? RemoveUser(int userId, string connectionId);
    }
}
