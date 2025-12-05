using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces.StatusInterfaces;
using UP_Student_Management.Classes.Models.StatusModels;

namespace UP_Student_Management.Classes.Context.StatusContext
{
    public class OvzContext : Ovz, IOvz
    {
        public List<OvzContext> AllOvz()
        {
            List<OvzContext> allOvz = new List<OvzContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader data = Connection.Query("SELECT * FROM `Ovz`", connection);

            while (data.Read())
            {
                OvzContext ovz = new OvzContext
                {
                    Id = data.GetInt32(0),
                    StudentId = data.GetInt32(1),
                    Prikaz = data.GetString(2),
                    Note = data.IsDBNull(3) ? null : data.GetString(3),
                    DocumentPath = data.IsDBNull(4) ? null : data.GetString(4),
                    StartDate = data.GetDateTime(5),
                    EndDate = data.IsDBNull(6) ? (DateTime?)null : data.GetDateTime(6)
                };
                allOvz.Add(ovz);
            }
            return allOvz;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();
            if (Update)
            {
                Connection.Query($@"
                UPDATE `Ovz` 
                SET 
                    `StudentId` = {this.StudentId},
                    `Prikaz` = '{this.Prikaz}',
                    `Note` = '{this.Note}',
                    `DocumentPath` = '{this.DocumentPath}',
                    `StartDate` = '{this.StartDate:yyyy-MM-dd HH:mm:ss}',
                    `EndDate` = {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")}
                WHERE `Id` = {this.Id}", connection);
            }
            else
            {
                Connection.Query($@"
                INSERT INTO `Ovz` 
                (`StudentId`, `Prikaz`, `Note`, `DocumentPath`, `StartDate`, `EndDate`) 
                VALUES 
                ({this.StudentId}, '{this.Prikaz}', '{this.Note}', '{this.DocumentPath}', '{this.StartDate:yyyy-MM-dd HH:mm:ss}', {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")})", connection);
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();
            Connection.Query($"DELETE FROM `Ovz` WHERE `Id` = {this.Id}", connection);
            MySqlConnection.ClearPool(connection);
        }
    }
}
