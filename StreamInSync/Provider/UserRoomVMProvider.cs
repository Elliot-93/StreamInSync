using StreamInSync.Models;
using StreamInSync.Services;
using System.Collections.Generic;
using System.Linq;

namespace StreamInSync.Provider
{
    public class UserRoomVMProvider : IUserRoomVMProvider
    {
        private readonly IRoomService roomService;

        public UserRoomVMProvider()
        {
            roomService = new RoomService();
        }

        public IEnumerable<UserRoomVM> UsersRooms(int userId)
        {
            return roomService
                .GetUsersRooms(userId)
                .Select(r => new UserRoomVM(r, BuildRoomLink(r)));
        }

        private string BuildRoomLink(Room room)
        {
            return "/room?roomid=" + room.RoomId;
        }
    }
}