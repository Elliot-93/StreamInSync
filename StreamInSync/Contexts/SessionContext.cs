using Newtonsoft.Json;
using StreamInSync.Models;
using System;
using System.Web;
using System.Web.Security;

namespace StreamInSync.Contexts
{
    public class SessionContext : ISessionContext
    {
        public void SetAuthCookie(User user)
        {
            var userData = JsonConvert.SerializeObject(user);

            var authTicket = new FormsAuthenticationTicket(1, user.Username, DateTime.Now, DateTime.Now.AddYears(1), true, userData);

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Expires = authTicket.Expiration
            };

            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public User GetUser()
        {
            User userData = null;

                HttpCookie cookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
                if (cookie != null)
                {
                    var authTicket = FormsAuthentication.Decrypt(cookie.Value);

                    userData = JsonConvert.DeserializeObject<User>(authTicket.UserData);
                }

            return userData;
        }

        //public void UpdateRoomAuthCookie(int roomId)
        //{
        //    HttpCookie cookie = HttpContext.Current.Request.Cookies["Rooms"];
        //    if (cookie != null)
        //    {
        //        var authTicket = FormsAuthentication.Decrypt(cookie.Value);

        //        authenticatedRooms = JsonConvert.DeserializeObject<User>(authTicket.UserData);
        //    }
        //}
    }
}