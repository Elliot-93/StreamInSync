using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using StreamInSync.Services;
using System.Linq;
using System.Web.Security;
using StreamInSync.Models;
using Newtonsoft.Json;

namespace StreamInSync.Hubs
{
    public class RoomHub : Hub
    {
        private readonly IRoomService roomService;
        private readonly IUserService userService;

        public RoomHub()
        {
            roomService = new RoomService();
            userService = new UserService();
        }

        public void JoinRoom(int roomId)
        {
            Groups.Add(Context.ConnectionId, roomId.ToString());

            roomService.AddUser(roomId, GetUser().Id, Context.ConnectionId);

            Clients.Group(roomId.ToString()).updateRoomUsers(userService.GetUsers(roomId).ToArray());
        }

        public void LeaveRoom(int roomId)
        {
            Groups.Add(Context.ConnectionId, roomId.ToString());

            roomService.AddUser(roomId, GetUser().Id, Context.ConnectionId);

            Clients.Group(roomId.ToString()).updateRoomUsers(userService.GetUsers(roomId).ToArray());
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var user = GetUser();

            var roomId = roomService.RemoveUser(GetUser().Id, Context.ConnectionId);

            if (roomId != null)
            {
                Clients.Group(roomId.ToString()).updateRoomUsers(userService.GetUsers((int)roomId).ToArray());
            }

            return base.OnDisconnected(stopCalled);
        }

        private User GetUser()
        {
            var authCookie = Context.RequestCookies[FormsAuthentication.FormsCookieName];
            User user = null;
            if (authCookie != null)
            {
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                user = JsonConvert.DeserializeObject<User>(authTicket.UserData);
            }
            return user;
        }
    }
}