using System.Collections.Generic;

namespace StreamInSync.Models
{
    public class Homepage
    {
        public Homepage(IEnumerable<RoomInfo> userRooms)
        {
            UserRooms = userRooms;
        }

        public IEnumerable<RoomInfo> UserRooms { get; private set; }
    }
}