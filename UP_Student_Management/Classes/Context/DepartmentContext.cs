using MySql.Data.MySqlClient;
using System;
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

            string query = "SELECT * FROM `Departments` ORDER BY Name";
            MySqlDataReader data = Connection.Query(query, connection);

            while (data.Read())
            {
                DepartmentContext department = new DepartmentContext
                {
                    Id = data.GetInt32(0),
                    Name = data.GetString(1)
                };
                allDepartments.Add(department);
            }

            data.Close();
            connection.Close();
            return allDepartments;
        }

        // Метод для получения отделения по ID
        public DepartmentContext GetById(int id)
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "SELECT * FROM `Departments` WHERE Id = @id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                MySqlDataReader data = command.ExecuteReader();

                if (data.Read())
                {
                    return new DepartmentContext
                    {
                        Id = data.GetInt32(0),
                        Name = data.GetString(1)
                    };
                }

                data.Close();
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения отделения по ID: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        // Метод для поиска отделения по имени
        public List<DepartmentContext> SearchByName(string name)
        {
            List<DepartmentContext> departments = new List<DepartmentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "SELECT * FROM `Departments` WHERE Name LIKE @name ORDER BY Name";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", $"%{name}%");

                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    departments.Add(new DepartmentContext
                    {
                        Id = data.GetInt32(0),
                        Name = data.GetString(1)
                    });
                }

                data.Close();
                return departments;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка поиска отделения: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                if (Update)
                {
                    string query = "UPDATE `Departments` SET Name = @name WHERE Id = @id";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", this.Name);
                    command.Parameters.AddWithValue("@id", this.Id);

                    command.ExecuteNonQuery();
                }
                else
                {
                    string query = "INSERT INTO `Departments` (Name) VALUES (@name)";
                    MySqlCommand command = new MySqlCommand(query, connection);
                    command.Parameters.AddWithValue("@name", this.Name);

                    command.ExecuteNonQuery();

                    // Получаем ID вставленной записи
                    this.Id = (int)command.LastInsertedId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения отделения: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "DELETE FROM `Departments` WHERE Id = @id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", this.Id);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка удаления отделения: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        // Метод для проверки существования отделения
        public bool Exists(int id)
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "SELECT COUNT(*) FROM `Departments` WHERE Id = @id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        // Метод для проверки существования отделения по имени
        public bool ExistsByName(string name, int? excludeId = null)
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "SELECT COUNT(*) FROM `Departments` WHERE Name = @name";

                if (excludeId.HasValue)
                {
                    query += " AND Id != @excludeId";
                }

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@name", name);

                if (excludeId.HasValue)
                {
                    command.Parameters.AddWithValue("@excludeId", excludeId.Value);
                }

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
            catch
            {
                return false;
            }
            finally
            {
                connection.Close();
            }
        }

        // Метод для получения количества студентов в отделении
        public int GetStudentCount()
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "SELECT COUNT(*) FROM Students WHERE DepartmentId = @departmentId";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@departmentId", this.Id);

                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения количества студентов: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        // Метод для получения статистики по отделению
        public DepartmentStatistics GetStatistics()
        {
            var statistics = new DepartmentStatistics();
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                // Общее количество студентов
                string queryCount = @"
                    SELECT COUNT(*) as TotalStudents,
                           SUM(CASE WHEN isBudget = 1 THEN 1 ELSE 0 END) as BudgetStudents,
                           SUM(CASE WHEN Sex = 'М' THEN 1 ELSE 0 END) as MaleStudents
                    FROM Students
                    WHERE DepartmentId = @departmentId";

                MySqlCommand command = new MySqlCommand(queryCount, connection);
                command.Parameters.AddWithValue("@departmentId", this.Id);

                MySqlDataReader data = command.ExecuteReader();

                if (data.Read())
                {
                    statistics.TotalStudents = data.GetInt32("TotalStudents");
                    statistics.BudgetStudents = data.GetInt32("BudgetStudents");
                    statistics.ContractStudents = statistics.TotalStudents - statistics.BudgetStudents;
                    statistics.MaleStudents = data.GetInt32("MaleStudents");
                    statistics.FemaleStudents = statistics.TotalStudents - statistics.MaleStudents;
                }

                data.Close();

                // Студенты по годам
                string queryYears = @"
                    SELECT YearReceipts, COUNT(*) as Count
                    FROM Students
                    WHERE DepartmentId = @departmentId
                    GROUP BY YearReceipts
                    ORDER BY YearReceipts DESC";

                command = new MySqlCommand(queryYears, connection);
                command.Parameters.AddWithValue("@departmentId", this.Id);

                data = command.ExecuteReader();

                while (data.Read())
                {
                    int year = data.GetInt32("YearReceipts");
                    int count = data.GetInt32("Count");
                    statistics.StudentsByYear[year] = count;
                }

                data.Close();

                return statistics;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения статистики отделения: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }
    }

    // Класс для статистики отделения
    public class DepartmentStatistics
    {
        public int TotalStudents { get; set; }
        public int BudgetStudents { get; set; }
        public int ContractStudents { get; set; }
        public int MaleStudents { get; set; }
        public int FemaleStudents { get; set; }
        public Dictionary<int, int> StudentsByYear { get; set; } = new Dictionary<int, int>();
    }
}