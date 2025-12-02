using MySql.Data.MySqlClient;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces;
using UP_Student_Management.Classes.Models;

namespace UP_Student_Management.Classes.Context
{
    public class DepartmentContext : Department, IDepartment
    {
        public List<DepartmentContext> AllDepartments()
        {
            List<DepartmentContext> allDepartments = new List<DepartmentContext>();
            MySqlConnection connection = Connection.OpenConnection();
            MySqlDataReader data = Connection.Query("SELECT * FROM `Departments`", connection);

            while (data.Read())
            {
                DepartmentContext department = new DepartmentContext
                {
                    Id = data.GetInt32(0),
                    Name = data.GetString(1)
                };
                allDepartments.Add(department);
            }
            return allDepartments;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();
            if (Update)
            {
                Connection.Query("UPDATE `Departments` " +
                                    "SET " +
                                        $"`Name` = '{this.Name}' " +
                                    $"WHERE `Id` = {this.Id}", connection);
            }
            else
            {
                Connection.Query("INSERT INTO `Departments` " +
                                    "(`Name`) " +
                                 $"VALUES ('{this.Name}')", connection);
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();
            Connection.Query($"DELETE FROM `Departments` WHERE `Id` = {this.Id}", connection);
            MySqlConnection.ClearPool(connection);
        }
    }
}
