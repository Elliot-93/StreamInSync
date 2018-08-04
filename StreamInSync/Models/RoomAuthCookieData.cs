using System.Collections.Generic;

namespace StreamInSync.Models
{
    public class RoomAuthCookieData
    {
        public RoomAuthCookieData(User user, List<int> authenticatedRoomIds)
        {
            User = user;
            AuthenticatedRoomIds = authenticatedRoomIds;
        }

        public User User { get; private set; }

        public List<int> AuthenticatedRoomIds { get; set; }
    }
}