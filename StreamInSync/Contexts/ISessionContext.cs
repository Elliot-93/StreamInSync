using StreamInSync.Models;

namespace StreamInSync.Contexts
{
    public interface ISessionContext
    {
        User GetUser();
        void SetAuthCookie(User user);
    }
}