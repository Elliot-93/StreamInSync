namespace StreamInSync.Models
{
    public class User
    {
        public User(int id, string username)
        {
            Id = id;
            Username = username;
        }

        public int Id { get; private set; }

        public string Username { get; private set; }
    }
}