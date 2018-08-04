using System.Collections.Generic;

namespace StreamInSync.Models
{
    public class RoomVM
    {
        public RoomVM(Room room, IEnumerable<User> users)
        {
            Users = users;
        }

        public Room Room { get; private set; }
        
        public IEnumerable<User> Users { get; private set; }
    }
}