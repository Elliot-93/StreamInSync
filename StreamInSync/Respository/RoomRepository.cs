using StreamInSync.Models;
using StreamInSync.Respository.Interfaces;
using System;
using System.Data;
using System.Data.SqlClient;

namespace StreamInSync.Respository
{
    public class RoomRepository : IRoomRepository
    {
        private const string Server = @"Server=localhost\SQLEXPRESS03;Database=StreamInSync;Trusted_Connection=True;";

        public Room Create(CreateRoomVM newRoom, User user)
        {
            using (var sqlConn = new SqlConnection(Server))
            {
                using (var cmd = new SqlCommand("dbo.CreateRoom", sqlConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Name", newRoom.Name));
                    cmd.Parameters.Add(new SqlParameter("@Password", newRoom.Password));
                    cmd.Parameters.Add(new SqlParameter("@UserId", user.Id));
                    cmd.Parameters.Add(new SqlParameter("@ProgrammeName", newRoom.ProgrammeName));
                    cmd.Parameters.Add(new SqlParameter("@ProgrammeRuntimeInSeconds", newRoom.RuntimeInSeconds));
                    cmd.Parameters.Add(new SqlParameter("@ProgrammeStartTime", newRoom.ProgrammeStartTime));

                    sqlConn.Open();

                    var roomId = (int)cmd.ExecuteScalar();

                    if (roomId > 0)
                    {
                        return new Room(
                            roomId, 
                            newRoom.Name, 
                            user, 
                            newRoom.ProgrammeName,
                            newRoom.RuntimeInSeconds,
                            newRoom.ProgrammeStartTime);
                    }

                    return null;
                }
            }
        }

        public Room Get(string name, string password)
        {
            using (var sqlConn = new SqlConnection(Server))
            {
                using (var cmd = new SqlCommand("dbo.GetRoomByRoomNameAndPassword", sqlConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@Roomname", name));
                    cmd.Parameters.Add(new SqlParameter("@Password", password));

                    sqlConn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            return new Room(
                                (int)reader["RoomId"],
                                (string)reader["RoomName"],
                                new User((int)reader["UserId"], (string)reader["Username"]),
                                (string)reader["ProgrammeName"],
                                new TimeSpan(0, 0, (int)reader["ProgrammeRuntimeInSeconds"]),
                                (DateTime)reader["ProgrammeStartTime"]);
                        }
                        else
                        {
                            return null;
                        }
                    }

                }
            }
        }

        public Room Get(int roomId)
        {
            using (var sqlConn = new SqlConnection(Server))
            {
                using (var cmd = new SqlCommand("dbo.GetRoomByRoomId", sqlConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@RoomId", roomId));

                    sqlConn.Open();

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            reader.Read();

                            return new Room(
                                (int)reader["RoomId"],
                                (string)reader["RoomName"],
                                new User((int)reader["UserId"], (string)reader["Username"]),
                                (string)reader["ProgrammeName"],
                                new TimeSpan(0, 0, (int)reader["ProgrammeRuntimeInSeconds"]),
                                (DateTime)reader["ProgrammeStartTime"]);
                        }
                        else
                        {
                            return null;
                        }
                    }

                }
            }
        }

        public void AddUser(int roomId, int userId, string connectionId)
        {
            using (var sqlConn = new SqlConnection(Server))
            {
                using (var cmd = new SqlCommand("dbo.AddUserToRoom", sqlConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@RoomId", roomId));
                    cmd.Parameters.Add(new SqlParameter("@UserId", userId));
                    cmd.Parameters.Add(new SqlParameter("@ConnectionId", connectionId));

                    sqlConn.Open();

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public int? RemoveUser(int userId, string connectionId)
        {
            using (var sqlConn = new SqlConnection(Server))
            {
                using (var cmd = new SqlCommand("dbo.RemoveUserFromRoom", sqlConn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@UserId", userId));
                    cmd.Parameters.Add(new SqlParameter("@ConnectionId", connectionId));

                    sqlConn.Open();

                    var result = (int)cmd.ExecuteScalar();

                    return result == 0 ? (int?)null : result;
                }
            }
        }
    }
}