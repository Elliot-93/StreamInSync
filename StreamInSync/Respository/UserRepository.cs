using StreamInSync.Models;
using StreamInSync.Respository.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace StreamInSync.Respository
{
    public class UserRepository : IUserRepository
    {
        private const string Server = @"Server=localhost\SQLEXPRESS03;Database=StreamInSync;Trusted_Connection=True;";

        public bool Create(RegisterVM newUser)
        {
            using (var sqlConn = new SqlConnection(Server))
            {
                using (var cmd = new SqlCommand("dbo.CreateUser", sqlConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Username", newUser.Username));
                    cmd.Parameters.Add(new SqlParameter("@Password", newUser.Password));

                    sqlConn.Open();

                    return (int)cmd.ExecuteScalar() == 1;
                }
            }
        }

        public User Get(string username, string password)
        {
            using (var sqlConn = new SqlConnection(Server))
            {
                using (var cmd = new SqlCommand("dbo.GetUserByUsernameAndPassword", sqlConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Username", username));
                    cmd.Parameters.Add(new SqlParameter("@Password", password));

                    sqlConn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            return new User((int)reader["Id"], (string)reader["Username"]);                        
                        }
                        else
                        {
                            return null;
                        }
                    }

                }
            }
        }

        public IList<User> GetUsers(int roomId)
        {
            using (var sqlConn = new SqlConnection(Server))
            {
                using (var cmd = new SqlCommand("dbo.GetUsersInRoom", sqlConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@RoomId", roomId));

                    sqlConn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        var users = new List<User>();

                        while (reader.Read())
                        {
                            users.Add(new User((int)reader["Id"], (string)reader["Username"]));
                        }

                        return users;
                    }

                }
            }
        }
    }
}