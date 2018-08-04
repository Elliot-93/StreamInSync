using StreamInSync.Contexts;
using StreamInSync.Models;
using StreamInSync.Services;
using System.Web.Mvc;
using System.Web.Security;

namespace StreamInSync.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly IUserService userService;
        private readonly ISessionContext sessionContext;

        public AuthenticationController()
        {
            userService = new UserService();
            sessionContext = new SessionContext();
        }

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogIn(LoginVM loginCredentials, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var authenticatedUser = userService.Get(loginCredentials.Username, loginCredentials.Password);
                if (authenticatedUser != null)
                {
                    sessionContext.SetAuthCookie(authenticatedUser);

                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Home");
                }

                loginCredentials = new LoginVM { Username = loginCredentials.Username, Failed = true };
                return View(loginCredentials);
            }
            
            return View(loginCredentials);
        }

        public ActionResult LogOut()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
    }
}