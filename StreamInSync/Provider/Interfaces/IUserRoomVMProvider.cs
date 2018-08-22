using System.Collections.Generic;
using StreamInSync.Models;

namespace StreamInSync.Provider
{
    public interface IUserRoomVMProvider
    {
        IEnumerable<UserRoomVM> UsersRooms(int userId);
    }
}