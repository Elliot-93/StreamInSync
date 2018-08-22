namespace StreamInSync.Hubs
{
    using System.Threading.Tasks;
    using Microsoft.AspNet.SignalR;
    using StreamInSync.Services;
    using System.Linq;
    using System.Web.Security;
    using Newtonsoft.Json;

    public class RoomHub : Hub
    {
        private readonly IRoomService roomService;

        public RoomHub()
        {
            roomService = new RoomService();
        }

        public void JoinRoom(int roomId)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return;
            }

            Groups.Add(Context.ConnectionId, roomId.ToString());

            roomService.AddUser(roomId, userId.Value, Context.ConnectionId);

            Clients
                .Group(roomId.ToString())
                .updateRoomUsers(roomService.Get(roomId).Members.Select(m => new { m.Username }).ToArray());
        }

        public void LeaveRoom(int roomId)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return;
            }

            Groups.Add(Context.ConnectionId, roomId.ToString());

            roomService.AddUser(roomId, userId.Value, Context.ConnectionId);

            Clients.Group(roomId.ToString()).updateRoomUsers(roomService.Get(roomId).Members.ToArray());
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return base.OnDisconnected(stopCalled);
            }

            var roomId = roomService.RemoveUser(userId.Value, Context.ConnectionId);

            if (roomId != null)
            {
                Clients.Group(roomId.ToString()).updateRoomUsers(roomService.Get(roomId.Value).Members.ToArray());
            }

            return base.OnDisconnected(stopCalled);
        }

        private int? GetUserId()
        {
            var authCookie = Context.RequestCookies[FormsAuthentication.FormsCookieName];
            int? user = null;
            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                user = JsonConvert.DeserializeObject<int>(authTicket.UserData);
            }
            return user;
        }
    }
}