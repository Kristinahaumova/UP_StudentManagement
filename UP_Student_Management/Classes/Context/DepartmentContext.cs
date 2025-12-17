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

            string query = "SELECT * FROM `Departments` ORDER BY Name";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                using (MySqlDataReader data = Connection.Query(query, connection))
                {
                    while (data.Read())
                    {
                        DepartmentContext department = new DepartmentContext
                        {
                            Id = data.GetInt32(0),
                            Name = data.GetString(1)
                        };
                        allDepartments.Add(department);
                    }
                }
            }
            return allDepartments;
        }

        // Метод для получения отделения по ID
        public DepartmentContext GetById(int id)
        {
            string query = "SELECT * FROM `Departments` WHERE Id = @id";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);

                        using (MySqlDataReader data = command.ExecuteReader())
                        {
                            if (data.Read())
                            {
                                return new DepartmentContext
                                {
                                    Id = data.GetInt32(0),
                                    Name = data.GetString(1)
                                };
                            }
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка получения отделения по ID: {ex.Message}");
                }
            }
        }

        // Метод для поиска отделения по имени
        public List<DepartmentContext> SearchByName(string name)
        {
            List<DepartmentContext> departments = new List<DepartmentContext>();

            string query = "SELECT * FROM `Departments` WHERE Name LIKE @name ORDER BY Name";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", $"%{name}%");

                        using (MySqlDataReader data = command.ExecuteReader())
                        {
                            while (data.Read())
                            {
                                departments.Add(new DepartmentContext
                                {
                                    Id = data.GetInt32(0),
                                    Name = data.GetString(1)
                                });
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка поиска отделения: {ex.Message}");
                }
            }

            return departments;
        }

        public void Save(bool Update = false)
        {
            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                try
                {
                    if (Update)
                    {
                        string query = "UPDATE `Departments` SET Name = @name WHERE Id = @id";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", this.Name);
                            command.Parameters.AddWithValue("@id", this.Id);
                            command.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        string query = "INSERT INTO `Departments` (Name) VALUES (@name)";
                        using (MySqlCommand command = new MySqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@name", this.Name);
                            command.ExecuteNonQuery();
                            this.Id = (int)command.LastInsertedId;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка сохранения отделения: {ex.Message}");
                }
            }
        }

        public void Delete()
        {
            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                try
                {
                    string query = "DELETE FROM `Departments` WHERE Id = @id";
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", this.Id);
                        command.ExecuteNonQuery();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка удаления отделения: {ex.Message}");
                }
            }
        }

        // Метод для проверки существования отделения
        public bool Exists(int id)
        {
            string query = "SELECT COUNT(*) FROM `Departments` WHERE Id = @id";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) return false;

                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@id", id);
                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        // Метод для проверки существования отделения по имени
        public bool ExistsByName(string name, int? excludeId = null)
        {
            string query = "SELECT COUNT(*) FROM `Departments` WHERE Name = @name";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) return false;

                try
                {
                    if (excludeId.HasValue)
                    {
                        query += " AND Id != @excludeId";
                    }

                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@name", name);

                        if (excludeId.HasValue)
                        {
                            command.Parameters.AddWithValue("@excludeId", excludeId.Value);
                        }

                        int count = Convert.ToInt32(command.ExecuteScalar());
                        return count > 0;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        // Метод для получения количества студентов в отделении
        public int GetStudentCount()
        {
            string query = "SELECT COUNT(*) FROM Students WHERE DepartmentId = @departmentId";

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                try
                {
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@departmentId", this.Id);
                        return Convert.ToInt32(command.ExecuteScalar());
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка получения количества студентов: {ex.Message}");
                }
            }
        }

        // Метод для получения статистики по отделению
        public DepartmentStatistics GetStatistics()
        {
            var statistics = new DepartmentStatistics();

            using (MySqlConnection connection = Connection.OpenConnection())
            {
                if (connection == null) throw new Exception("Не удалось установить соединение с базой данных");

                try
                {
                    // Общее количество студентов
                    string queryCount = @"
                        SELECT COUNT(*) as TotalStudents,
                               SUM(CASE WHEN isBudget = 1 THEN 1 ELSE 0 END) as BudgetStudents,
                               SUM(CASE WHEN Sex = 'М' THEN 1 ELSE 0 END) as MaleStudents
                        FROM Students
                        WHERE DepartmentId = @departmentId";

                    using (MySqlCommand command = new MySqlCommand(queryCount, connection))
                    {
                        command.Parameters.AddWithValue("@departmentId", this.Id);

                        using (MySqlDataReader data = command.ExecuteReader())
                        {
                            if (data.Read())
                            {
                                statistics.TotalStudents = data.GetInt32("TotalStudents");
                                statistics.BudgetStudents = data.GetInt32("BudgetStudents");
                                statistics.ContractStudents = statistics.TotalStudents - statistics.BudgetStudents;
                                statistics.MaleStudents = data.GetInt32("MaleStudents");
                                statistics.FemaleStudents = statistics.TotalStudents - statistics.MaleStudents;
                            }
                        }
                    }

                    // Студенты по годам
                    string queryYears = @"
                        SELECT YearReceipts, COUNT(*) as Count
                        FROM Students
                        WHERE DepartmentId = @departmentId
                        GROUP BY YearReceipts
                        ORDER BY YearReceipts DESC";

                    using (MySqlCommand cmdYears = new MySqlCommand(queryYears, connection))
                    {
                        cmdYears.Parameters.AddWithValue("@departmentId", this.Id);

                        using (MySqlDataReader yearsData = cmdYears.ExecuteReader())
                        {
                            while (yearsData.Read())
                            {
                                int year = yearsData.GetInt32("YearReceipts");
                                int count = yearsData.GetInt32("Count");
                                statistics.StudentsByYear[year] = count;
                            }
                        }
                    }

                    return statistics;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Ошибка получения статистики отделения: {ex.Message}");
                }
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
