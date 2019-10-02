namespace StreamInSync.Provider
{
    using StreamInSync.Services;
    using StreamInSync.Models;
    using System.Collections.Generic;
    using System.Linq;

    public class RoomSummaryVmProvider : IRoomSummaryVmProvider
    {
        private readonly IRoomService roomService;

        public RoomSummaryVmProvider()
        {
            roomService = new RoomService();
        }

        public IEnumerable<RoomSummaryVM> BuildRoomViewModels(IEnumerable<Room> rooms)
        {
            return rooms.Select(r => new RoomSummaryVM(r, BuildRoomLink(r)));
        }

        private string BuildRoomLink(Room room)
        {
            return "/room?roomid=" + room.RoomId;
        }
    }
}