using System.Collections.Generic;
using StreamInSync.Models;

namespace StreamInSync.Provider
{
    public interface IRoomSummaryVmProvider
    {
        IEnumerable<RoomSummaryVM> BuildRoomViewModels(IEnumerable<Room> rooms);
    }
}