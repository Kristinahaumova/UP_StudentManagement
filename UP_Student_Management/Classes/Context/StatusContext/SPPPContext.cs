using MySql.Data.MySqlClient;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces.StatusInterfaces;
using UP_Student_Management.Classes.Models.StatusModels;

namespace UP_Student_Management.Classes.Context.StatusContext
{
    public class SPPPContext : SPPP, ISPPP
    {
        public List<SPPPContext> AllSPPP()
        {
            List<SPPPContext> allSPPP = new List<SPPPContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader data = Connection.Query("SELECT * FROM `SPPP`", connection);

            while (data.Read())
            {
                SPPPContext sppp = new SPPPContext
                {
                    Id = data.GetInt32(0),
                    StudentId = data.GetInt32(1),
                    CallReason = data.IsDBNull(2) ? null : data.GetString(2),
                    EmployeesPresent = data.IsDBNull(3) ? null : data.GetString(3),
                    RepresentativesPresent = data.IsDBNull(4) ? null : data.GetString(4),
                    ReasonCall = data.IsDBNull(5) ? null : data.GetString(5),
                    Decision = data.IsDBNull(6) ? null : data.GetString(6),
                    Note = data.IsDBNull(7) ? null : data.GetString(7),
                    DocumentPath = data.IsDBNull(8) ? null : data.GetString(8),
                    Date = data.GetDateTime(9)
                };
                allSPPP.Add(sppp);
            }
            return allSPPP;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();
            if (Update)
            {
                Connection.Query($@"
                UPDATE `SPPP` 
                SET 
                    `StudentId` = {this.StudentId},
                    `CallReason` = '{this.CallReason}',
                    `EmployeesPresent` = '{this.EmployeesPresent}',
                    `RepresentativesPresent` = '{this.RepresentativesPresent}',
                    `ReasonCall` = '{this.ReasonCall}',
                    `Decision` = '{this.Decision}',
                    `Note` = '{this.Note}',
                    `DocumentPath` = '{this.DocumentPath}',
                    `Date` = '{this.Date:yyyy-MM-dd HH:mm:ss}'
                WHERE `Id` = {this.Id}", connection);
            }
            else
            {
                Connection.Query($@"
                INSERT INTO `SPPP` 
                (`StudentId`, `CallReason`, `EmployeesPresent`, `RepresentativesPresent`, `ReasonCall`, `Decision`, `Note`, `DocumentPath`, `Date`) 
                VALUES 
                ({this.StudentId}, '{this.CallReason}', '{this.EmployeesPresent}', '{this.RepresentativesPresent}', '{this.ReasonCall}', '{this.Decision}', '{this.Note}', '{this.DocumentPath}', '{this.Date:yyyy-MM-dd HH:mm:ss}')", connection);
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();
            Connection.Query($"DELETE FROM `SPPP` WHERE `Id` = {this.Id}", connection);
            MySqlConnection.ClearPool(connection);
        }
    }
}
