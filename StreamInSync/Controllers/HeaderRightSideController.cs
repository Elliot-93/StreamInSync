using StreamInSync.Contexts;
using System.Web.Mvc;

namespace StreamInSync.Controllers
{
    public class HeaderRightSideController : Controller
    {
        private readonly ISessionContext sessionContext;

        public HeaderRightSideController()
        {
            sessionContext = new SessionContext();
        }

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View("LoggedIn", sessionContext.GetUser());
            }

            return View("LoggedOut");
        }
    }
}