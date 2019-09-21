namespace StreamInSync.Provider
{
    using StreamInSync.Services;
    using StreamInSync.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class RoomSummaryProvider : IRoomSummaryProvider
    {
        private readonly IRoomService roomService;

        public RoomSummaryProvider()
        {
            roomService = new RoomService();
        }

        public IEnumerable<RoomSummaryVM> UsersRooms(int userId)
        {
            return roomService
                .GetUsersRooms(userId)
                .Select(r => new RoomSummaryVM(r, BuildRoomLink(r)));
        }

        private string BuildRoomLink(Room room)
        {
            return "/room?roomid=" + room.RoomId;
        }
    }
}