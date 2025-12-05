using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces.StatusInterfaces;
using UP_Student_Management.Classes.Models.StatusModels;

namespace UP_Student_Management.Classes.Context.StatusContext
{
    public class HostelContext : Hostel, IHostel
    {
        public List<HostelContext> AllHostel()
        {
            List<HostelContext> allHostel = new List<HostelContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader data = Connection.Query("SELECT * FROM `Hostel`", connection);

            while (data.Read())
            {
                HostelContext hostel = new HostelContext
                {
                    Id = data.GetInt32(0),
                    StudentId = data.GetInt32(1),
                    RoomId = data.GetInt32(2),
                    Note = data.IsDBNull(3) ? null : data.GetString(3),
                    DocumentPath = data.IsDBNull(4) ? null : data.GetString(4),
                    StartDate = data.GetDateTime(5),
                    EndDate = data.IsDBNull(6) ? (DateTime?)null : data.GetDateTime(6)
                };
                allHostel.Add(hostel);
            }
            return allHostel;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();
            if (Update)
            {
                Connection.Query($@"
                UPDATE `Hostel` 
                SET 
                    `StudentId` = {this.StudentId},
                    `RoomId` = {this.RoomId},
                    `Note` = '{this.Note}',
                    `DocumentPath` = '{this.DocumentPath}',
                    `StartDate` = '{this.StartDate:yyyy-MM-dd HH:mm:ss}',
                    `EndDate` = {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")}
                WHERE `Id` = {this.Id}", connection);
            }
            else
            {
                Connection.Query($@"
                INSERT INTO `Hostel` 
                (`StudentId`, `RoomId`, `Note`, `DocumentPath`, `StartDate`, `EndDate`) 
                VALUES 
                ({this.StudentId}, {this.RoomId}, '{this.Note}', '{this.DocumentPath}', '{this.StartDate:yyyy-MM-dd HH:mm:ss}', {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")})", connection);
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();
            Connection.Query($"DELETE FROM `Hostel` WHERE `Id` = {this.Id}", connection);
            MySqlConnection.ClearPool(connection);
        }
    }
}
