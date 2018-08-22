using StreamInSync.Contexts;
using StreamInSync.Models;
using StreamInSync.Provider;
using System.Linq;
using System.Web.Mvc;

namespace StreamInSync.Controllers
{
    public class HomeController : Controller
    {
        private readonly ISessionContext sessionContext;
        private readonly IUserRoomVMProvider userRoomVMProvider;

        public HomeController()
        {
            sessionContext = new SessionContext();
            userRoomVMProvider = new UserRoomVMProvider();
        }

        public ActionResult Index()
        {
            var user = sessionContext.GetUser();
            var userRooms = Enumerable.Empty<UserRoomVM>();

            if (user != null)
            {
                userRooms = userRoomVMProvider.UsersRooms(user.UserId);
            }

            return View(new HomeVM(userRooms));
        }
    }
}