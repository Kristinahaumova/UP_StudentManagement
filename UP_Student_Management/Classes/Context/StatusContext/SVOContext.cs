using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces.StatusInterfaces;
using UP_Student_Management.Classes.Models.StatusModels;

namespace UP_Student_Management.Classes.Context.StatusContext
{
    public class SVOContext : SVO, ISVO
    {
        public List<SVOContext> AllSVO()
        {
            List<SVOContext> allSVO = new List<SVOContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader data = Connection.Query("SELECT * FROM `SVO`", connection);

            while (data.Read())
            {
                SVOContext svo = new SVOContext
                {
                    Id = data.GetInt32(0),
                    StudentId = data.GetInt32(1),
                    Prikaz = data.GetString(2),
                    DocumentPath = data.IsDBNull(3) ? null : data.GetString(3),
                    StartDate = data.GetDateTime(4),
                    EndDate = data.IsDBNull(5) ? (DateTime?)null : data.GetDateTime(5)
                };
                allSVO.Add(svo);
            }
            return allSVO;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();
            if (Update)
            {
                Connection.Query($@"
                UPDATE `SVO` 
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
                INSERT INTO `SVO` 
                (`StudentId`, `Prikaz`, `DocumentPath`, `StartDate`, `EndDate`) 
                VALUES 
                ({this.StudentId}, '{this.Prikaz}', '{this.DocumentPath}', '{this.StartDate:yyyy-MM-dd HH:mm:ss}', {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")})", connection);
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();
            Connection.Query($"DELETE FROM `SVO` WHERE `Id` = {this.Id}", connection);
            MySqlConnection.ClearPool(connection);
        }
    }
}
