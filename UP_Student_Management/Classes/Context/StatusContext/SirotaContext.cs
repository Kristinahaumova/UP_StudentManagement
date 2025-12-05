using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces.StatusInterfaces;
using UP_Student_Management.Classes.Models.StatusModels;

namespace UP_Student_Management.Classes.Context.StatusContext
{
    public class SirotaContext : Sirota, ISirota
    {
        public List<SirotaContext> AllSirota()
        {
            List<SirotaContext> allSirota = new List<SirotaContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader data = Connection.Query("SELECT * FROM `Sirota`", connection);

            while (data.Read())
            {
                SirotaContext sirota = new SirotaContext
                {
                    Id = data.GetInt32(0),
                    StudentId = data.GetInt32(1),
                    Prikaz = data.GetString(2),
                    Note = data.IsDBNull(3) ? null : data.GetString(3),
                    DocumentPath = data.IsDBNull(4) ? null : data.GetString(4),
                    StartDate = data.GetDateTime(5),
                    EndDate = data.IsDBNull(6) ? (DateTime?)null : data.GetDateTime(6)
                };
                allSirota.Add(sirota);
            }
            return allSirota;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();
            if (Update)
            {
                Connection.Query($@"
                UPDATE `Sirota` 
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
                INSERT INTO `Sirota` 
                (`StudentId`, `Prikaz`, `Note`, `DocumentPath`, `StartDate`, `EndDate`) 
                VALUES 
                ({this.StudentId}, '{this.Prikaz}', '{this.Note}', '{this.DocumentPath}', '{this.StartDate:yyyy-MM-dd HH:mm:ss}', {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")})", connection);
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();
            Connection.Query($"DELETE FROM `Sirota` WHERE `Id` = {this.Id}", connection);
            MySqlConnection.ClearPool(connection);
        }
    }
}
