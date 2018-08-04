using StreamInSync.Models;
using StreamInSync.Respository;
using StreamInSync.Respository.Interfaces;
using System.Collections.Generic;

namespace StreamInSync.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository userRepository;

        public UserService()
        {
            userRepository = new UserRepository();
        }

        public bool Create(RegisterVM newUser)
        {
            return userRepository.Create(newUser);
        }

        public User Get(string username, string password)
        {
            return userRepository.Get(username, password);
        }

        public IEnumerable<User> GetUsers(int roomId)
        {
            return userRepository.GetUsers(roomId);
        }
    }
}
