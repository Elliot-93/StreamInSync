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
        private readonly IUserService userService;

        public RoomController()
        {
            sessionContext = new SessionContext();
            roomService = new RoomService();
            userService = new UserService();
        }

        public ActionResult Index(int roomId)
        {
            //get room if publci fine. If private check on auth token - authenticatedPrivateRoomIds

            var room = roomService.Get(roomId);

            if (room != null)
            {
                return View(new RoomVM(room, userService.GetUsers(roomId)));
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
                var room = roomService.Get(joinRoom.Name, joinRoom.Password);

                if (room != null)
                {
                    return RedirectToAction("Index", new { roomId = room.Id });
                }
            }

            return View(joinRoom);
        }
    }
}