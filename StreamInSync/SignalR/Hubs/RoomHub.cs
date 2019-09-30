namespace StreamInSync.Hubs
{
    using System.Threading.Tasks;
    using Microsoft.AspNet.SignalR;
    using StreamInSync.Services;
    using System.Linq;
    using System.Web.Security;
    using Newtonsoft.Json;
    using System;
    using StreamInSync.Models;
    using StreamInSync.Enums;

    public class RoomHub : Hub
    {
        private readonly IRoomService roomService;

        public RoomHub()
        {
            roomService = new RoomService();
        }

        public void JoinRoom(JoinRoom joinRoom)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return;
            }

            Groups.Add(Context.ConnectionId, joinRoom.RoomId.ToString());

            roomService.AddUser(joinRoom.RoomId, userId.Value, Context.ConnectionId, UnixTimeStampToDateTime(joinRoom.LastUpdated));

            Clients
                .Group(joinRoom.RoomId.ToString())
                .updateRoomUsers(GetSerializableRoomMembers(joinRoom.RoomId));
        }

        //todo: Handle errors, return error message to user and log
        public void UpdateServerProgrammeTime(ProgrammeTimeUpdate programmeTimeUpdate)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return;
            }

            if (roomService.UpdateRoomMember(new RoomMemberUpdate(userId.Value, programmeTimeUpdate)))
            {
                Clients
                    .Group(programmeTimeUpdate.RoomId.ToString())
                    .updateRoomUsers(GetSerializableRoomMembers(programmeTimeUpdate.RoomId));
            }
        }

        //todo: add button to leave room
        public void LeaveRoom(int roomId)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return;
            }

            Groups.Add(Context.ConnectionId, roomId.ToString());

            roomService.RemoveUser(userId.Value, Context.ConnectionId);

            Clients.Group(roomId.ToString()).updateRoomUsers(GetSerializableRoomMembers(roomId));
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var userId = GetUserId();

            if (userId == null)
            {
                return base.OnDisconnected(stopCalled);
            }

            var roomId = roomService.DisconnectUser(userId.Value, Context.ConnectionId);

            if (roomId != null)
            {
                Clients.Group(roomId.ToString()).updateRoomUsers(GetSerializableRoomMembers(roomId.Value));
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

        private object GetSerializableRoomMembers(int roomId)
        {
            return roomService.Get(roomId).Members
                .Where(m => 
                    !string.IsNullOrEmpty(m.ConnectionId) 
                    || (m.PlayStatus != PlayStatus.NotStarted  && (DateTime.UtcNow - m.LastUpdated).TotalMinutes < 15 ))
                .Select(m =>
                    new
                    {
                        m.UserId,
                        m.Username,
                        m.LastUpdated,
                        m.ProgrammeTimeSecs,
                        m.InBreak,
                        BreakTimeSecs = m.BreakTimeSecs ?? 0,
                        m.PlayStatus
                    }).ToArray();
        }

        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTimeStamp);
        }
    }
}