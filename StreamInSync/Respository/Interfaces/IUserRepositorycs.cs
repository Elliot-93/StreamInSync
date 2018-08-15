using StreamInSync.Models;

namespace StreamInSync.Respository.Interfaces
{
    interface IUserRepository
    {
        bool Create(RegisterVM newUser);
        User Get(string username, string password);
        User Get(int userId);
    }
}
