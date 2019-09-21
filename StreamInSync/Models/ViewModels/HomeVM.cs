using System.Collections.Generic;

namespace StreamInSync.Models
{
    public class HomeVM
    {
        public HomeVM(IEnumerable<RoomSummaryVM> userRooms)
        {
            UserRooms = userRooms;
        }

        public IEnumerable<RoomSummaryVM> UserRooms { get; private set; }
    }
}