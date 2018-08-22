using System.Collections.Generic;

namespace StreamInSync.Models
{
    public class HomeVM
    {
        public HomeVM(IEnumerable<UserRoomVM> userRooms)
        {
            UserRooms = userRooms;
        }

        public IEnumerable<UserRoomVM> UserRooms { get; private set; }
    }
}