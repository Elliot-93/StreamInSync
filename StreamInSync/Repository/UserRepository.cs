namespace StreamInSync.Repository
{
    using StreamInSync.Data;
    using StreamInSync.Models;
    using StreamInSync.Repository.Interfaces;
    using System.Linq;

    public class UserRepository : IUserRepository
    {
        private readonly SiteDbContext dbContext;

        public UserRepository()
        {
            dbContext = new SiteDbContext();
        }

        public bool Create(RegisterVM newUser)
        {
            if (dbContext.Users.Any(u => u.Username == newUser.Username))
            {
                return false;
            }

            dbContext.Users.Add(new User { Username = newUser.Username, Password = newUser.Password });
            return dbContext.SaveChanges() == 1;
        }

        public User Get(string username, string password)
        {
            return dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
        }

        public User Get(int userId)
        {
            return dbContext.Users.Find(userId);
        }
    }
}