using Newtonsoft.Json;
using StreamInSync.Contexts;
using StreamInSync.Models;
using StreamInSync.Services;
using System.Web.Mvc;

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
            //get room if public fine. If private check on auth token - authenticatedPrivateRoomIds

            var room = roomService.Get(roomId);

            if (room != null)
            {
                var roomJson = new { room.Id };

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
                    return RedirectToAction("Index", new { roomId = room.Id });
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
                    return RedirectToAction("Index", new { roomId = room.Id });
                }
            }

            return View(joinRoom);
        }
    }
}