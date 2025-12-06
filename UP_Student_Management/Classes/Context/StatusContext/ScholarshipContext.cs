using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces.StatusInterfaces;
using UP_Student_Management.Classes.Models.StatusModels;

namespace UP_Student_Management.Classes.Context.StatusContext
{
    public class ScholarshipContext : Scholarship, IScholarship
    {
        public List<ScholarshipContext> AllScholarship()
        {
            List<ScholarshipContext> allScholarship = new List<ScholarshipContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader data = Connection.Query("SELECT * FROM `Scholarship`", connection);

            while (data.Read())
            {
                ScholarshipContext scholarship = new ScholarshipContext
                {
                    Id = data.GetInt32(0),
                    StudentId = data.GetInt32(1),
                    Prikaz = data.GetString(2),
                    DocumentPath = data.IsDBNull(3) ? null : data.GetString(3),
                    StartDate = data.GetDateTime(4),
                    EndDate = data.IsDBNull(5) ? (DateTime?)null : data.GetDateTime(5)
                };
                allScholarship.Add(scholarship);
            }
            return allScholarship;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();
            if (Update)
            {
                Connection.Query($@"
                UPDATE `Scholarship` 
                SET 
                    `StudentId` = {this.StudentId},
                    `Prikaz` = '{this.Prikaz}',
                    `DocumentPath` = '{this.DocumentPath}',
                    `StartDate` = '{this.StartDate:yyyy-MM-dd HH:mm:ss}',
                    `EndDate` = {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")}
                WHERE `Id` = {this.Id}", connection);
            }
            else
            {
                Connection.Query($@"
                INSERT INTO `Scholarship` 
                (`StudentId`, `Prikaz`, `DocumentPath`, `StartDate`, `EndDate`) 
                VALUES 
                ({this.StudentId}, '{this.Prikaz}', '{this.DocumentPath}', '{this.StartDate:yyyy-MM-dd HH:mm:ss}', {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")})", connection);
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();
            Connection.Query($"DELETE FROM `Scholarship` WHERE `Id` = {this.Id}", connection);
            MySqlConnection.ClearPool(connection);
        }
    }
}
