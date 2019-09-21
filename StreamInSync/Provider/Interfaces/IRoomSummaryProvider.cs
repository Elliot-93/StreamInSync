using System.Collections.Generic;
using StreamInSync.Models;

namespace StreamInSync.Provider
{
    public interface IRoomSummaryProvider
    {
        IEnumerable<RoomSummaryVM> UsersRooms(int userId);
    }
}