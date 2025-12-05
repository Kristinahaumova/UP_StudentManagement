using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces.StatusInterfaces;
using UP_Student_Management.Classes.Models.StatusModels;

namespace UP_Student_Management.Classes.Context.StatusContext
{
    public class InvalidContext : Invalid, IInvalid
    {
        public List<InvalidContext> AllInvalid()
        {
            List<InvalidContext> allInvalid = new List<InvalidContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader data = Connection.Query("SELECT * FROM `Invalid`", connection);

            while (data.Read())
            {
                InvalidContext invalid = new InvalidContext
                {
                    Id = data.GetInt32(0),
                    StudentId = data.GetInt32(1),
                    Prikaz = data.GetString(2),
                    Note = data.IsDBNull(3) ? null : data.GetString(3),
                    InvalidType = data.GetString(4),
                    DocumentPath = data.IsDBNull(5) ? null : data.GetString(5),
                    StartDate = data.GetDateTime(6),
                    EndDate = data.IsDBNull(7) ? (DateTime?)null : data.GetDateTime(7)
                };
                allInvalid.Add(invalid);
            }
            return allInvalid;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();
            if (Update)
            {
                Connection.Query($@"
                UPDATE `Invalid` 
                SET 
                    `StudentId` = {this.StudentId},
                    `Prikaz` = '{this.Prikaz}',
                    `Note` = '{this.Note}',
                    `InvalidType` = '{this.InvalidType}',
                    `DocumentPath` = '{this.DocumentPath}',
                    `StartDate` = '{this.StartDate:yyyy-MM-dd HH:mm:ss}',
                    `EndDate` = {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")}
                WHERE `Id` = {this.Id}", connection);
            }
            else
            {
                Connection.Query($@"
                INSERT INTO `Invalid` 
                (`StudentId`, `Prikaz`, `Note`, `InvalidType`, `DocumentPath`, `StartDate`, `EndDate`) 
                VALUES 
                ({this.StudentId}, '{this.Prikaz}', '{this.Note}', '{this.InvalidType}', '{this.DocumentPath}', '{this.StartDate:yyyy-MM-dd HH:mm:ss}', {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")})", connection);
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();
            Connection.Query($"DELETE FROM `Invalid` WHERE `Id` = {this.Id}", connection);
            MySqlConnection.ClearPool(connection);
        }
    }
}
