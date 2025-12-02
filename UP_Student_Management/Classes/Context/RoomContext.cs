using MySql.Data.MySqlClient;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces;
using UP_Student_Management.Classes.Models;

namespace UP_Student_Management.Classes.Context
{
    public class RoomContext : Room, IRoom
    {
        public List<RoomContext> AllRooms()
        {
            List<RoomContext> allRooms = new List<RoomContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader data = Connection.Query("SELECT * FROM `Rooms`", connection);

            while (data.Read())
            {
                RoomContext room = new RoomContext
                {
                    Id = data.GetInt32(0),
                    Name = data.GetString(1),
                    Capacity = data.GetInt32(2)
                };
                allRooms.Add(room);
            }
            return allRooms;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();
            if (Update)
            {
                Connection.Query("UPDATE `Rooms` " +
                                    "SET " +
                                        $"`Name` = '{this.Name}', " +
                                        $"`Capacity` = '{this.Capacity}' " +
                                    $"WHERE `Id` = {this.Id}", connection);
            }
            else
            {
                Connection.Query("INSERT INTO `Rooms` " +
                                    "(`Name`, `Capacity`) " +
                                 $"VALUES ('{this.Name}', '{this.Capacity}')", connection);
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();
            Connection.Query($"DELETE FROM `Rooms` WHERE `Id` = {this.Id}", connection);
            MySqlConnection.ClearPool(connection);
        }
    }
}
