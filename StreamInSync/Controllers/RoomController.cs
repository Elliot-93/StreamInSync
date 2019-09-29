using Newtonsoft.Json;
using StreamInSync.Contexts;
using StreamInSync.Models;
using StreamInSync.Services;
using System.Web.Mvc;
using System.Linq;

namespace StreamInSync.Controllers
{
    [Authorize]
    public class RoomController : Controller
    {
        private readonly ISessionContext sessionContext;
        private readonly IRoomService roomService;

        public RoomController()
        {
            sessionContext = new SessionContext();
            roomService = new RoomService();
        }

        public ActionResult Index(int roomId)
        {
            //todo: get room if public fine. If private check on auth token - authenticatedPrivateRoomIds

            var room = roomService.Get(roomId);
            var userId = sessionContext.GetUser().UserId;
            var existingUserRoomData = room.Members.FirstOrDefault(m => m.UserId == userId);

            if (room != null)
            {
                var roomJson = new
                {
                    room.RoomId,
                    UserId = userId,
                    TotalRuntimeSeconds = room.Runtime.TotalSeconds,
                    ProgrammeTimeSecs = existingUserRoomData?.ProgrammeTimeSecs ?? 0,
                    existingUserRoomData.PlayStatus,
                    LastUpdatedTime = existingUserRoomData.LastUpdated,
                    existingUserRoomData.InBreak,
                    existingUserRoomData.BreakTimeSecs
                };

                return View(new RoomVM(room, JsonConvert.SerializeObject(roomJson)));
            }

            return View("~/Views/Shared/Error");
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CreateRoomVM newRoom)
        {
            if (ModelState.IsValid)
            {
                var room = roomService.Create(newRoom, sessionContext.GetUser());

                if (room != null)
                {
                    // ToDo: do this in routing intialisation?
                    return RedirectToAction("Index", new { roomId = room.RoomId });
                }
            }

            return View(newRoom);
        }

        public ActionResult Join()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Join(JoinRoomVM joinRoom)
        {
            if (ModelState.IsValid)
            {
                var room = roomService.Get(joinRoom.InviteCode, joinRoom.Password);

                if (room != null)
                {
                    return RedirectToAction("Index", new { roomId = room.RoomId });
                }
            }

            return View(joinRoom);
        }
    }
}