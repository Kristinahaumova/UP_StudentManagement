using MySql.Data.MySqlClient;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces;
using UP_Student_Management.Classes.Models;

namespace UP_Student_Management.Classes.Context
{
    public class UserContext : User, IUser
    {
        public List<UserContext> AllUsers()
        {
            List<UserContext> allUsers = new List<UserContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader data = Connection.Query("SELECT * FROM `Users`", connection);

            while (data.Read())
            {
                UserContext user = new UserContext
                {
                    Id = data.GetInt32(0),
                    Name = data.GetString(1),
                    Password = data.GetString(2),
                    isAdmin = data.GetInt32(3)
                };
                allUsers.Add(user);
            }
            return allUsers;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();
            if (Update)
            {
                Connection.Query("UPDATE `Users` " +
                                    "SET " +
                                        $"`Name` = '{this.Name}', " +
                                        $"`Password` = '{this.Password}', " +
                                        $"`isAdmin` = '{this.isAdmin}' " +
                                    $"WHERE `Id` = {this.Id}", connection);
            }
            else
            {
                Connection.Query("INSERT INTO `Users` " +
                                    "(`Name`, `Password`, `isAdmin`) " +
                                 $"VALUES ('{this.Name}', '{this.Password}', '{this.isAdmin}')", connection);
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();
            Connection.Query($"DELETE FROM `Users` WHERE `Id` = {this.Id}", connection);
            MySqlConnection.ClearPool(connection);
        }
    }
}
