using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces;
using UP_Student_Management.Classes.Models;

namespace UP_Student_Management.Classes.Context
{
    public class StudentContext : Student,IStudent
    {
        // Возвращает всех студентов
        public List<StudentContext> AllStudents()
        {
            List<StudentContext> allStudents = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    Id, Firstname, Surname, BirthDate, Patronomyc, 
                    Sex, Phone, Education, GroupName, isBudget,
                    YearReceipts, YearFinish, DeductionsInfo, 
                    DataDeductions, Note, FilePath, DepartmentId, RoomId
                FROM `Students`
                ORDER BY Surname, Firstname";

            MySqlDataReader data = Connection.Query(query, connection);

            while (data.Read())
            {
                StudentContext student = new StudentContext
                {
                    Id = data.GetInt32("Id"),
                    Firstname = data.GetString("Firstname"),
                    Surname = data.GetString("Surname"),
                    BirthDate = data.GetDateTime("BirthDate"),
                    Sex = data.GetString("Sex"),
                    Education = data.GetString("Education"),
                    GroupName = GetSafeString(data, "GroupName"),
                    isBudget = data.GetInt32("isBudget"),
                    DepartmentId = data.GetInt32("DepartmentId")
                };

                // Обработка полей YEAR - читаем как int и преобразуем в DateTime
                if (!data.IsDBNull(data.GetOrdinal("YearReceipts")))
                {
                    int yearReceipts = data.GetInt32("YearReceipts");
                    student.YearReceipts = new DateTime(yearReceipts, 1, 1);
                }
                else
                {
                    student.YearReceipts = DateTime.MinValue;
                }

                if (!data.IsDBNull(data.GetOrdinal("YearFinish")))
                {
                    int yearFinish = data.GetInt32("YearFinish");
                    student.YearFinish = new DateTime(yearFinish, 1, 1);
                }
                else
                {
                    student.YearFinish = DateTime.MinValue;
                }

                // Обработка nullable полей
                if (!data.IsDBNull(data.GetOrdinal("Patronomyc")))
                    student.Patronomyc = data.GetString("Patronomyc");

                if (!data.IsDBNull(data.GetOrdinal("Phone")))
                    student.Phone = data.GetString("Phone");

                if (!data.IsDBNull(data.GetOrdinal("DeductionsInfo")))
                    student.DeductionsInfo = data.GetString("DeductionsInfo");

                if (!data.IsDBNull(data.GetOrdinal("DataDeductions")))
                    student.DataDeductions = data.GetDateTime("DataDeductions");

                if (!data.IsDBNull(data.GetOrdinal("Note")))
                    student.Note = data.GetString("Note");

                if (!data.IsDBNull(data.GetOrdinal("FilePath")))
                    student.FilePath = data.GetString("FilePath");

                if (!data.IsDBNull(data.GetOrdinal("RoomId")))
                    student.RoomId = data.GetInt32("RoomId");

                allStudents.Add(student);
            }

            data.Close();
            connection.Close();
            return allStudents;
        }

        // Поиск студента по ID
        public StudentContext FindById(int id)
        {
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    Id, Firstname, Surname, BirthDate, Patronomyc, 
                    Sex, Phone, Education, GroupName, isBudget,
                    YearReceipts, YearFinish, DeductionsInfo, 
                    DataDeductions, Note, FilePath, DepartmentId, RoomId
                FROM `Students`
                WHERE Id = @id";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", id);

            MySqlDataReader data = command.ExecuteReader();

            if (data.Read())
            {
                StudentContext student = new StudentContext
                {
                    Id = data.GetInt32("Id"),
                    Firstname = data.GetString("Firstname"),
                    Surname = data.GetString("Surname"),
                    BirthDate = data.GetDateTime("BirthDate"),
                    Sex = data.GetString("Sex"),
                    Education = data.GetString("Education"),
                    GroupName = GetSafeString(data, "GroupName"),
                    isBudget = data.GetInt32("isBudget"),
                    DepartmentId = data.GetInt32("DepartmentId")
                };

                // Обработка полей YEAR
                if (!data.IsDBNull(data.GetOrdinal("YearReceipts")))
                {
                    int yearReceipts = data.GetInt32("YearReceipts");
                    student.YearReceipts = new DateTime(yearReceipts, 1, 1);
                }
                else
                {
                    student.YearReceipts = DateTime.MinValue;
                }

                if (!data.IsDBNull(data.GetOrdinal("YearFinish")))
                {
                    int yearFinish = data.GetInt32("YearFinish");
                    student.YearFinish = new DateTime(yearFinish, 1, 1);
                }
                else
                {
                    student.YearFinish = DateTime.MinValue;
                }

                // Обработка nullable полей
                if (!data.IsDBNull(data.GetOrdinal("Patronomyc")))
                    student.Patronomyc = data.GetString("Patronomyc");

                if (!data.IsDBNull(data.GetOrdinal("Phone")))
                    student.Phone = data.GetString("Phone");

                if (!data.IsDBNull(data.GetOrdinal("DeductionsInfo")))
                    student.DeductionsInfo = data.GetString("DeductionsInfo");

                if (!data.IsDBNull(data.GetOrdinal("DataDeductions")))
                    student.DataDeductions = data.GetDateTime("DataDeductions");

                if (!data.IsDBNull(data.GetOrdinal("Note")))
                    student.Note = data.GetString("Note");

                if (!data.IsDBNull(data.GetOrdinal("FilePath")))
                    student.FilePath = data.GetString("FilePath");

                if (!data.IsDBNull(data.GetOrdinal("RoomId")))
                    student.RoomId = data.GetInt32("RoomId");

                data.Close();
                connection.Close();
                return student;
            }

            data.Close();
            connection.Close();
            return null;
        }

        // Поиск студентов по фамилии
        public List<StudentContext> FindBySurname(string surname)
        {
            List<StudentContext> students = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    Id, Firstname, Surname, BirthDate, Patronomyc, 
                    Sex, Phone, Education, GroupName, isBudget,
                    YearReceipts, YearFinish, DeductionsInfo, 
                    DataDeductions, Note, FilePath, DepartmentId, RoomId
                FROM `Students`
                WHERE Surname LIKE @surname
                ORDER BY Surname, Firstname";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@surname", $"%{surname}%");

            MySqlDataReader data = command.ExecuteReader();

            while (data.Read())
            {
                StudentContext student = new StudentContext
                {
                    Id = data.GetInt32("Id"),
                    Firstname = data.GetString("Firstname"),
                    Surname = data.GetString("Surname"),
                    BirthDate = data.GetDateTime("BirthDate"),
                    Sex = data.GetString("Sex"),
                    Education = data.GetString("Education"),
                    GroupName = GetSafeString(data, "GroupName"),
                    isBudget = data.GetInt32("isBudget"),
                    DepartmentId = data.GetInt32("DepartmentId")
                };

                // Обработка полей YEAR
                if (!data.IsDBNull(data.GetOrdinal("YearReceipts")))
                {
                    int yearReceipts = data.GetInt32("YearReceipts");
                    student.YearReceipts = new DateTime(yearReceipts, 1, 1);
                }

                if (!data.IsDBNull(data.GetOrdinal("YearFinish")))
                {
                    int yearFinish = data.GetInt32("YearFinish");
                    student.YearFinish = new DateTime(yearFinish, 1, 1);
                }

                if (!data.IsDBNull(data.GetOrdinal("Patronomyc")))
                    student.Patronomyc = data.GetString("Patronomyc");

                if (!data.IsDBNull(data.GetOrdinal("Phone")))
                    student.Phone = data.GetString("Phone");

                if (!data.IsDBNull(data.GetOrdinal("DeductionsInfo")))
                    student.DeductionsInfo = data.GetString("DeductionsInfo");

                if (!data.IsDBNull(data.GetOrdinal("DataDeductions")))
                    student.DataDeductions = data.GetDateTime("DataDeductions");

                if (!data.IsDBNull(data.GetOrdinal("Note")))
                    student.Note = data.GetString("Note");

                if (!data.IsDBNull(data.GetOrdinal("FilePath")))
                    student.FilePath = data.GetString("FilePath");

                if (!data.IsDBNull(data.GetOrdinal("RoomId")))
                    student.RoomId = data.GetInt32("RoomId");

                students.Add(student);
            }

            data.Close();
            connection.Close();
            return students;
        }

        // Сохранение студента (вставка или обновление)
        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                if (Update)
                {
                    // Обновление существующей записи
                    string query = @"
                        UPDATE `Students` 
                        SET 
                            Firstname = @firstname,
                            Surname = @surname,
                            BirthDate = @birthdate,
                            Patronomyc = @patronomyc,
                            Sex = @sex,
                            Phone = @phone,
                            Education = @education,
                            GroupName = @groupName,
                            isBudget = @isBudget,
                            YearReceipts = @yearReceipts,
                            YearFinish = @yearFinish,
                            DeductionsInfo = @deductionsInfo,
                            DataDeductions = @dataDeductions,
                            Note = @note,
                            FilePath = @filePath,
                            DepartmentId = @departmentId,
                            RoomId = @roomId
                        WHERE Id = @id";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    AddParameters(command);
                    command.Parameters.AddWithValue("@id", this.Id);

                    command.ExecuteNonQuery();
                }
                else
                {
                    // Вставка новой записи
                    string query = @"
                        INSERT INTO `Students` 
                        (Firstname, Surname, BirthDate, Patronomyc, Sex, Phone, 
                         Education, GroupName, isBudget, YearReceipts, YearFinish,
                         DeductionsInfo, DataDeductions, Note, FilePath, DepartmentId, RoomId) 
                        VALUES 
                        (@firstname, @surname, @birthdate, @patronomyc, @sex, @phone,
                         @education, @groupName, @isBudget, @yearReceipts, @yearFinish,
                         @deductionsInfo, @dataDeductions, @note, @filePath, @departmentId, @roomId)";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    AddParameters(command);

                    command.ExecuteNonQuery();

                    // Получаем ID вставленной записи
                    this.Id = (int)command.LastInsertedId;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения студента: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        // Вспомогательный метод для безопасного чтения строк
        private string GetSafeString(MySqlDataReader data, string columnName)
        {
            int ordinal = data.GetOrdinal(columnName);
            if (!data.IsDBNull(ordinal))
            {
                return data.GetString(ordinal);
            }
            return null;
        }

        // Вспомогательный метод для добавления параметров
        private void AddParameters(MySqlCommand command)
        {
            command.Parameters.AddWithValue("@firstname", this.Firstname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@surname", this.Surname ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@birthdate", this.BirthDate);
            command.Parameters.AddWithValue("@patronomyc", this.Patronomyc ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@sex", this.Sex ?? "М");
            command.Parameters.AddWithValue("@phone", this.Phone ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@education", this.Education ?? "11");
            command.Parameters.AddWithValue("@groupName", this.GroupName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@isBudget", this.isBudget);

            // Годы как int
            command.Parameters.AddWithValue("@yearReceipts", this.YearReceipts.Year);
            command.Parameters.AddWithValue("@yearFinish", this.YearFinish.Year);

            command.Parameters.AddWithValue("@deductionsInfo", this.DeductionsInfo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@dataDeductions",
                this.DataDeductions != DateTime.MinValue ? (object)this.DataDeductions : DBNull.Value);
            command.Parameters.AddWithValue("@note", this.Note ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@filePath", this.FilePath ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@departmentId", this.DepartmentId);
            command.Parameters.AddWithValue("@roomId", this.RoomId.HasValue ? (object)this.RoomId.Value : DBNull.Value);
        }

        // Удаление студента
        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "DELETE FROM `Students` WHERE Id = @id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", this.Id);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка удаления студента: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        // Остальные методы с исправлением чтения YEAR полей...

        // Вспомогательный метод для маппинга данных из DataReader
        private StudentContext MapDataReaderToStudent(MySqlDataReader data)
        {
            StudentContext student = new StudentContext
            {
                Id = data.GetInt32("Id"),
                Firstname = data.GetString("Firstname"),
                Surname = data.GetString("Surname"),
                BirthDate = data.GetDateTime("BirthDate"),
                Sex = data.GetString("Sex"),
                Education = data.GetString("Education"),
                GroupName = GetSafeString(data, "GroupName"),
                isBudget = data.GetInt32("isBudget"),
                DepartmentId = data.GetInt32("DepartmentId")
            };

            // Обработка полей YEAR
            if (!data.IsDBNull(data.GetOrdinal("YearReceipts")))
            {
                int yearReceipts = data.GetInt32("YearReceipts");
                student.YearReceipts = new DateTime(yearReceipts, 1, 1);
            }

            if (!data.IsDBNull(data.GetOrdinal("YearFinish")))
            {
                int yearFinish = data.GetInt32("YearFinish");
                student.YearFinish = new DateTime(yearFinish, 1, 1);
            }

            if (!data.IsDBNull(data.GetOrdinal("Patronomyc")))
                student.Patronomyc = data.GetString("Patronomyc");

            if (!data.IsDBNull(data.GetOrdinal("Phone")))
                student.Phone = data.GetString("Phone");

            if (!data.IsDBNull(data.GetOrdinal("DeductionsInfo")))
                student.DeductionsInfo = data.GetString("DeductionsInfo");

            if (!data.IsDBNull(data.GetOrdinal("DataDeductions")))
                student.DataDeductions = data.GetDateTime("DataDeductions");

            if (!data.IsDBNull(data.GetOrdinal("Note")))
                student.Note = data.GetString("Note");

            if (!data.IsDBNull(data.GetOrdinal("FilePath")))
                student.FilePath = data.GetString("FilePath");

            if (!data.IsDBNull(data.GetOrdinal("RoomId")))
                student.RoomId = data.GetInt32("RoomId");

            return student;
        }

        // Остальные методы остаются без изменений, но нужно обновить их для использования MapDataReaderToStudent
        // Метод для получения студентов по группе (теперь по имени группы)
        public List<StudentContext> GetStudentsByGroup(string groupName)
        {
            List<StudentContext> students = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT * FROM `Students`
                WHERE GroupName = @groupName
                AND DataDeductions IS NULL
                ORDER BY Surname, Firstname";

            MySqlCommand command = new MySqlCommand(query, connection);
            command.Parameters.AddWithValue("@groupName", groupName);

            MySqlDataReader data = command.ExecuteReader();

            while (data.Read())
            {
                StudentContext student = MapDataReaderToStudent(data);
                students.Add(student);
            }

            data.Close();
            connection.Close();
            return students;
        }

        // Метод для получения списка всех уникальных групп
        public List<string> GetAllGroups()
        {
            List<string> groups = new List<string>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT DISTINCT GroupName 
                FROM `Students`
                WHERE GroupName IS NOT NULL
                AND GroupName != ''
                ORDER BY GroupName";

            MySqlDataReader data = Connection.Query(query, connection);

            while (data.Read())
            {
                groups.Add(data.GetString("GroupName"));
            }

            data.Close();
            connection.Close();
            return groups;
        }

        // Метод для получения неотчисленных студентов
        public List<StudentContext> GetActiveStudents()
        {
            List<StudentContext> students = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT * FROM `Students`
                WHERE DataDeductions IS NULL
                ORDER BY Surname, Firstname";

            MySqlDataReader data = Connection.Query(query, connection);

            while (data.Read())
            {
                StudentContext student = MapDataReaderToStudent(data);
                students.Add(student);
            }

            data.Close();
            connection.Close();
            return students;
        }

        // Метод для получения статистики
        public Dictionary<string, int> GetStatistics()
        {
            Dictionary<string, int> statistics = new Dictionary<string, int>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    COUNT(*) as Total,
                    SUM(CASE WHEN Sex = 'М' THEN 1 ELSE 0 END) as Male,
                    SUM(CASE WHEN Sex = 'Ж' THEN 1 ELSE 0 END) as Female,
                    SUM(CASE WHEN isBudget = 1 THEN 1 ELSE 0 END) as Budget,
                    SUM(CASE WHEN isBudget = 0 THEN 1 ELSE 0 END) as Contract,
                    SUM(CASE WHEN Education = '9' THEN 1 ELSE 0 END) as Education9,
                    SUM(CASE WHEN Education = '11' THEN 1 ELSE 0 END) as Education11,
                    SUM(CASE WHEN DataDeductions IS NULL THEN 1 ELSE 0 END) as Active,
                    SUM(CASE WHEN DataDeductions IS NOT NULL THEN 1 ELSE 0 END) as Deducted
                FROM `Students`";

            MySqlDataReader data = Connection.Query(query, connection);

            if (data.Read())
            {
                statistics.Add("Всего студентов", data.GetInt32("Total"));
                statistics.Add("Мужчин", data.GetInt32("Male"));
                statistics.Add("Женщин", data.GetInt32("Female"));
                statistics.Add("Бюджетников", data.GetInt32("Budget"));
                statistics.Add("Контрактников", data.GetInt32("Contract"));
                statistics.Add("С 9 классами", data.GetInt32("Education9"));
                statistics.Add("С 11 классами", data.GetInt32("Education11"));
                statistics.Add("Активных", data.GetInt32("Active"));
                statistics.Add("Отчисленных", data.GetInt32("Deducted"));
            }

            data.Close();
            connection.Close();
            return statistics;
        }
    }
}