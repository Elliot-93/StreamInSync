using StreamInSync.Contexts;
using StreamInSync.Models;
using StreamInSync.Provider;
using StreamInSync.Services;
using System.Linq;
using System.Web.Mvc;

namespace StreamInSync.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISessionContext sessionContext;
        private readonly IRoomService roomService;
        private readonly IRoomSummaryVmProvider userRoomVMProvider;

        public HomeController()
        {
            sessionContext = new SessionContext();
            roomService = new RoomService();
            userRoomVMProvider = new RoomSummaryVmProvider();
        }

        public ActionResult Index()
        {
            var user = sessionContext.GetUser();
            var userRooms = Enumerable.Empty<RoomSummaryVM>();

            if (user != null)
            {
                userRooms = userRoomVMProvider.BuildRoomViewModels(roomService.GetUsersRooms(user.UserId));
            }

            return View(new HomeVM(userRooms));
        }
    }
}