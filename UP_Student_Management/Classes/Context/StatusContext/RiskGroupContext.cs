using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces.StatusInterfaces;
using UP_Student_Management.Classes.Models.StatusModels;

namespace UP_Student_Management.Classes.Context.StatusContext
{
    public class RiskGroupContext : RiskGroup, IRiskGroup
    {
        public List<RiskGroupContext> AllRiskGroup()
        {
            List<RiskGroupContext> allRiskGroup = new List<RiskGroupContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader data = Connection.Query("SELECT * FROM `RiskGroup`", connection);

            while (data.Read())
            {
                RiskGroupContext riskGroup = new RiskGroupContext
                {
                    Id = data.GetInt32(0),
                    StudentId = data.GetInt32(1),
                    Type = data.GetString(2),
                    Note = data.IsDBNull(3) ? null : data.GetString(3),
                    RegistrationOsnovanie = data.IsDBNull(4) ? null : data.GetString(4),
                    RemovalOsnovanie = data.IsDBNull(5) ? null : data.GetString(5),
                    RegistrationReason = data.IsDBNull(6) ? null : data.GetString(6),
                    RemovalReason = data.IsDBNull(7) ? null : data.GetString(7),
                    DocumentPath = data.IsDBNull(8) ? null : data.GetString(8),
                    StartDate = data.GetDateTime(9),
                    EndDate = data.IsDBNull(10) ? (DateTime?)null : data.GetDateTime(10)
                };
                allRiskGroup.Add(riskGroup);
            }
            return allRiskGroup;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();
            if (Update)
            {
                Connection.Query($@"
                UPDATE `RiskGroup` 
                SET 
                    `StudentId` = {this.StudentId},
                    `Type` = '{this.Type}',
                    `Note` = '{this.Note}',
                    `RegistrationOsnovanie` = '{this.RegistrationOsnovanie}',
                    `RemovalOsnovanie` = '{this.RemovalOsnovanie}',
                    `RegistrationReason` = '{this.RegistrationReason}',
                    `RemovalReason` = '{this.RemovalReason}',
                    `DocumentPath` = '{this.DocumentPath}',
                    `StartDate` = '{this.StartDate:yyyy-MM-dd HH:mm:ss}',
                    `EndDate` = {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")}
                WHERE `Id` = {this.Id}", connection);
            }
            else
            {
                Connection.Query($@"
                INSERT INTO `RiskGroup` 
                (`StudentId`, `Type`, `Note`, `RegistrationOsnovanie`, `RemovalOsnovanie`, `RegistrationReason`, `RemovalReason`, `DocumentPath`, `StartDate`, `EndDate`) 
                VALUES 
                ({this.StudentId}, '{this.Type}', '{this.Note}', '{this.RegistrationOsnovanie}', '{this.RemovalOsnovanie}', '{this.RegistrationReason}', '{this.RemovalReason}', '{this.DocumentPath}', '{this.StartDate:yyyy-MM-dd HH:mm:ss}', {(this.EndDate.HasValue ? $"'{this.EndDate.Value:yyyy-MM-dd HH:mm:ss}'" : "NULL")})", connection);
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();
            Connection.Query($"DELETE FROM `RiskGroup` WHERE `Id` = {this.Id}", connection);
            MySqlConnection.ClearPool(connection);
        }
    }
}
