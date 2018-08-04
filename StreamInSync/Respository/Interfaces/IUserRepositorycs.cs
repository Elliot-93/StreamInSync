using StreamInSync.Models;
using System.Collections.Generic;

namespace StreamInSync.Respository.Interfaces
{
    interface IUserRepository
    {
        bool Create(RegisterVM newUser);
        User Get(string username, string password);
        IList<User> GetUsers(int roomId);
    }
}
