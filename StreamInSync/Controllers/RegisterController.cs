using StreamInSync.Models;
using StreamInSync.Services;
using System.Web.Mvc;

namespace StreamInSync.Controllers
{
    public class RegisterController : Controller
    {
        private readonly IUserService userService;

        public RegisterController()
        {
            this.userService = new UserService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(RegisterVM newUser)
        {
            if (ModelState.IsValid)
            {
                if (userService.Create(newUser))
                {
                    newUser = new RegisterVM { Success = true };
                }

                return View(newUser);
            }

            return View(newUser);
        }
    }
}