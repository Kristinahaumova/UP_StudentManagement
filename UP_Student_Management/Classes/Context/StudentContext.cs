using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces;
using UP_Student_Management.Classes.Models;

namespace UP_Student_Management.Classes.Context
{
    // Вложенный класс для работы с файлами
    public class StudentFileService
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public int StudentId { get; set; }
        public DateTime CreatedDate { get; set; }

        // Получить все файлы студента
        public List<StudentFileService> GetFilesByStudentId(int studentId)
        {
            List<StudentFileService> files = new List<StudentFileService>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT Id, FilePath, IdStudent, CreatedDate
                FROM StudentFiles
                WHERE IdStudent = @studentId
                ORDER BY CreatedDate DESC";

            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@studentId", studentId);

                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    files.Add(new StudentFileService
                    {
                        Id = data.GetInt32("Id"),
                        FilePath = data.GetString("FilePath"),
                        StudentId = data.GetInt32("IdStudent"),
                        CreatedDate = data.GetDateTime("CreatedDate")
                    });
                }

                data.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения файлов студента: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            return files;
        }

        // Добавить файл студента
        public void AddFile()
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = @"
                    INSERT INTO StudentFiles (FilePath, IdStudent)
                    VALUES (@filePath, @studentId)";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@filePath", this.FilePath);
                command.Parameters.AddWithValue("@studentId", this.StudentId);

                command.ExecuteNonQuery();

                this.Id = (int)command.LastInsertedId;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка добавления файла: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        // Удалить файл студента
        public void DeleteFile()
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "DELETE FROM StudentFiles WHERE Id = @id";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", this.Id);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка удаления файла: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        // Удалить все файлы студента
        public void DeleteAllFilesByStudentId(int studentId)
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "DELETE FROM StudentFiles WHERE IdStudent = @studentId";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@studentId", studentId);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка удаления файлов студента: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }
    }

    public class StudentContext : Student, IStudent
    {
        // Конструкторы
        public StudentContext()
        {
        }

        public StudentContext(Student student)
        {
            this.Id = student.Id;
            this.Firstname = student.Firstname;
            this.Surname = student.Surname;
            this.BirthDate = student.BirthDate;
            this.Patronomyc = student.Patronomyc;
            this.Sex = student.Sex;
            this.Phone = student.Phone;
            this.Education = student.Education;
            this.GroupName = student.GroupName;
            this.isBudget = student.isBudget;
            this.YearReceipts = student.YearReceipts;
            this.YearFinish = student.YearFinish;
            this.DeductionsInfo = student.DeductionsInfo;
            this.DataDeductions = student.DataDeductions;
            this.Note = student.Note;
            this.ParentsInfo = student.ParentsInfo;
            this.Penalties = student.Penalties;
            this.DepartmentId = student.DepartmentId;
        }

        #region Основные методы CRUD

        public List<StudentContext> AllStudents()
        {
            List<StudentContext> allStudents = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    Id, Firstname, Surname, BirthDate, Patronomyc, 
                    Sex, Phone, Education, GroupName, isBudget,
                    YearReceipts, YearFinish, DeductionsInfo, 
                    DataDeductions, Note, ParentsInfo, Penalties,
                    DepartmentId
                FROM `Students`
                ORDER BY Surname, Firstname";

            MySqlDataReader data = Connection.Query(query, connection);

            while (data.Read())
            {
                StudentContext student = MapDataReaderToStudent(data);
                allStudents.Add(student);
            }

            data.Close();
            connection.Close();
            return allStudents;
        }

        public void Save(bool Update = false)
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                if (Update)
                {
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
                            ParentsInfo = @parentsInfo,
                            Penalties = @penalties,
                            DepartmentId = @departmentId
                        WHERE Id = @id";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    AddParameters(command);
                    command.Parameters.AddWithValue("@id", this.Id);


                    command.ExecuteNonQuery();
                }
                else
                {
                    string query = @"
                        INSERT INTO `Students` 
                        (Firstname, Surname, BirthDate, Patronomyc, Sex, Phone, 
                         Education, GroupName, isBudget, YearReceipts, YearFinish,
                         DeductionsInfo, DataDeductions, Note, ParentsInfo, Penalties,
                         DepartmentId) 
                        VALUES 
                        (@firstname, @surname, @birthdate, @patronomyc, @sex, @phone,
                         @education, @groupName, @isBudget, @yearReceipts, @yearFinish,
                         @deductionsInfo, @dataDeductions, @note, @parentsInfo, @penalties,
                         @departmentId)";

                    MySqlCommand command = new MySqlCommand(query, connection);
                    AddParameters(command);

                    command.ExecuteNonQuery();

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

        public void Delete()
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                // Удаляем файлы студента
                DeleteStudentFilesFromDatabase();

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

        #endregion

        #region Методы для работы с файлами

        // Получить все файлы студента
        public List<StudentFileService> GetStudentFiles()
        {
            var fileService = new StudentFileService();
            return fileService.GetFilesByStudentId(this.Id);
        }

        // Добавить файл студента
        public void AddStudentFile(string filePath)
        {
            var studentFile = new StudentFileService
            {
                FilePath = filePath,
                StudentId = this.Id
            };

            studentFile.AddFile();
        }

        // Удалить конкретный файл студента
        public void DeleteStudentFile(int fileId)
        {
            var studentFile = new StudentFileService { Id = fileId };
            studentFile.DeleteFile();
        }

        // Удалить все файлы студента из базы данных
        public void DeleteStudentFilesFromDatabase()
        {
            var fileService = new StudentFileService();
            fileService.DeleteAllFilesByStudentId(this.Id);

            // Также удаляем физические файлы
            DeleteStudentPhysicalFiles();
        }

        // Сохранить файл студента (копирует файл и сохраняет в БД)
        public string SaveStudentFile(string sourceFilePath)
        {
            try
            {
                // Создаем папку для студента
                string studentFolder = GetStudentFolderPath();
                Directory.CreateDirectory(studentFolder);

                // Генерируем уникальное имя файла
                string fileName = Path.GetFileName(sourceFilePath);
                string destinationPath = Path.Combine(studentFolder, fileName);

                // Если файл уже существует, добавляем временную метку
                if (File.Exists(destinationPath))
                {
                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                    string extension = Path.GetExtension(fileName);
                    string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                    fileName = $"{fileNameWithoutExt}_{timestamp}{extension}";
                    destinationPath = Path.Combine(studentFolder, fileName);
                }

                // Копируем файл
                File.Copy(sourceFilePath, destinationPath, false);

                // Сохраняем информацию в базу данных
                AddStudentFile(destinationPath);

                return destinationPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка сохранения файла: {ex.Message}");
            }
        }

        // Получить информацию о файлах студента
        public List<StudentFileInfo> GetStudentFileInfos()
        {
            var files = GetStudentFiles();
            var fileInfos = new List<StudentFileInfo>();

            foreach (var file in files)
            {
                fileInfos.Add(new StudentFileInfo
                {
                    Id = file.Id,
                    FilePath = file.FilePath,
                    CreatedDate = file.CreatedDate,
                    FileName = Path.GetFileName(file.FilePath)
                });
            }

            return fileInfos;
        }

        // Удалить физические файлы студента
        private void DeleteStudentPhysicalFiles()
        {
            string studentFolder = GetStudentFolderPath();
            try
            {
                if (Directory.Exists(studentFolder))
                {
                    Directory.Delete(studentFolder, true);
                }
            }
            catch
            {
                // Игнорируем ошибки удаления папки
            }
        }

        // Получить путь к папке файлов студента
        public string GetStudentFolderPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "UP_Student_Management",
                "StudentFiles",
                this.Id.ToString()
            );
        }

        #endregion

        #region Статические методы для работы с файлами

        // Статический метод для получения файлов студента
        public static List<StudentFileService> GetFilesForStudent(int studentId)
        {
            var fileService = new StudentFileService();
            return fileService.GetFilesByStudentId(studentId);
        }

        // Статический метод для добавления файла студенту
        public static void AddFileToStudent(int studentId, string filePath)
        {
            var studentFile = new StudentFileService
            {
                FilePath = filePath,
                StudentId = studentId
            };
            studentFile.AddFile();
        }

        // Статический метод для удаления всех файлов студента
        public static void DeleteAllFilesForStudent(int studentId)
        {
            var fileService = new StudentFileService();
            fileService.DeleteAllFilesByStudentId(studentId);
        }

        // Статический метод для удаления конкретного файла
        public static void DeleteFile(int fileId)
        {
            var studentFile = new StudentFileService { Id = fileId };
            studentFile.DeleteFile();
        }

        #endregion

        #region Вспомогательные методы

        private string GetSafeString(MySqlDataReader data, string columnName)
        {
            int ordinal = data.GetOrdinal(columnName);
            if (!data.IsDBNull(ordinal))
            {
                return data.GetString(ordinal);
            }
            return null;
        }

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

            command.Parameters.AddWithValue("@yearReceipts", this.YearReceipts.Year);
            command.Parameters.AddWithValue("@yearFinish", this.YearFinish.Year);

            command.Parameters.AddWithValue("@deductionsInfo", this.DeductionsInfo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@dataDeductions",
                this.DataDeductions != DateTime.MinValue ? (object)this.DataDeductions : DBNull.Value);
            command.Parameters.AddWithValue("@note", this.Note ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@parentsInfo", this.ParentsInfo ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@penalties", this.Penalties ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@departmentId", this.DepartmentId);
        }

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

            if (!data.IsDBNull(data.GetOrdinal("ParentsInfo")))
                student.ParentsInfo = data.GetString("ParentsInfo");

            if (!data.IsDBNull(data.GetOrdinal("Penalties")))
                student.Penalties = data.GetString("Penalties");

            return student;
        }

        #endregion

        #region Методы поиска и фильтрации

        public List<StudentContext> SearchStudents(string searchTerm)
        {
            List<StudentContext> results = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    Id, Firstname, Surname, BirthDate, Patronomyc, 
                    Sex, Phone, Education, GroupName, isBudget,
                    YearReceipts, YearFinish, DeductionsInfo, 
                    DataDeductions, Note, ParentsInfo, Penalties,
                    DepartmentId
                FROM `Students`
                WHERE 
                    Firstname LIKE @searchTerm OR
                    Surname LIKE @searchTerm OR
                    Patronomyc LIKE @searchTerm OR
                    GroupName LIKE @searchTerm OR
                    Phone LIKE @searchTerm
                ORDER BY Surname, Firstname";

            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@searchTerm", $"%{searchTerm}%");

                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    StudentContext student = MapDataReaderToStudent(data);
                    results.Add(student);
                }

                data.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка поиска студентов: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            return results;
        }

        public StudentContext GetById(int id)
        {
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    Id, Firstname, Surname, BirthDate, Patronomyc, 
                    Sex, Phone, Education, GroupName, isBudget,
                    YearReceipts, YearFinish, DeductionsInfo, 
                    DataDeductions, Note, ParentsInfo, Penalties,
                    DepartmentId
                FROM `Students`
                WHERE Id = @id";

            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                MySqlDataReader data = command.ExecuteReader();

                if (data.Read())
                {
                    return MapDataReaderToStudent(data);
                }

                data.Close();
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения студента по ID: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public List<StudentContext> GetStudentsByDepartment(int departmentId)
        {
            List<StudentContext> students = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    Id, Firstname, Surname, BirthDate, Patronomyc, 
                    Sex, Phone, Education, GroupName, isBudget,
                    YearReceipts, YearFinish, DeductionsInfo, 
                    DataDeductions, Note, ParentsInfo, Penalties,
                    DepartmentId
                FROM `Students`
                WHERE DepartmentId = @departmentId
                ORDER BY Surname, Firstname";

            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@departmentId", departmentId);

                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    StudentContext student = MapDataReaderToStudent(data);
                    students.Add(student);
                }

                data.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения студентов по отделению: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            return students;
        }

        public List<StudentContext> GetStudentsByYear(int year)
        {
            List<StudentContext> students = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    Id, Firstname, Surname, BirthDate, Patronomyc, 
                    Sex, Phone, Education, GroupName, isBudget,
                    YearReceipts, YearFinish, DeductionsInfo, 
                    DataDeductions, Note, ParentsInfo, Penalties,
                    DepartmentId
                FROM `Students`
                WHERE YearReceipts = @year
                ORDER BY Surname, Firstname";

            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@year", year);

                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    StudentContext student = MapDataReaderToStudent(data);
                    students.Add(student);
                }

                data.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения студентов по году: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            return students;
        }

        public List<StudentContext> GetStudentsByGroup(string groupName)
        {
            List<StudentContext> students = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    Id, Firstname, Surname, BirthDate, Patronomyc, 
                    Sex, Phone, Education, GroupName, isBudget,
                    YearReceipts, YearFinish, DeductionsInfo, 
                    DataDeductions, Note, ParentsInfo, Penalties,
                    DepartmentId
                FROM `Students`
                WHERE GroupName LIKE @groupName
                ORDER BY Surname, Firstname";

            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@groupName", $"%{groupName}%");

                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    StudentContext student = MapDataReaderToStudent(data);
                    students.Add(student);
                }

                data.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения студентов по группе: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            return students;
        }

        public List<StudentContext> GetStudentsByFinanceType(bool isBudget)
        {
            List<StudentContext> students = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    Id, Firstname, Surname, BirthDate, Patronomyc, 
                    Sex, Phone, Education, GroupName, isBudget,
                    YearReceipts, YearFinish, DeductionsInfo, 
                    DataDeductions, Note, ParentsInfo, Penalties,
                    DepartmentId
                FROM `Students`
                WHERE isBudget = @isBudget
                ORDER BY Surname, Firstname";

            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@isBudget", isBudget ? 1 : 0);

                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    StudentContext student = MapDataReaderToStudent(data);
                    students.Add(student);
                }

                data.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения студентов по типу финансирования: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            return students;
        }

        #endregion

        #region Вспомогательные классы

        // Класс для информации о файле
        public class StudentFileInfo
        {
            public int Id { get; set; }
            public string FilePath { get; set; }
            public DateTime CreatedDate { get; set; }
            public string FileName { get; set; }
            public string FileExtension => Path.GetExtension(FilePath)?.ToLower();
            public string FileSize
            {
                get
                {
                    if (File.Exists(FilePath))
                    {
                        var fileInfo = new FileInfo(FilePath);
                        long bytes = fileInfo.Length;

                        if (bytes >= 1073741824)
                            return $"{bytes / 1073741824:F2} GB";
                        else if (bytes >= 1048576)
                            return $"{bytes / 1048576:F2} MB";
                        else if (bytes >= 1024)
                            return $"{bytes / 1024:F2} KB";
                        else
                            return $"{bytes} B";
                    }
                    return "N/A";
                }
            }
        }

        // Класс для статистики студентов
        public class StudentStatistics
        {
            public int TotalStudents { get; set; }
            public int BudgetStudents { get; set; }
            public int ContractStudents { get; set; }
            public int MaleStudents { get; set; }
            public int FemaleStudents { get; set; }
            public Dictionary<int, int> StudentsByYear { get; set; } = new Dictionary<int, int>();
            public Dictionary<int, int> StudentsByDepartment { get; set; } = new Dictionary<int, int>();
        }

        #endregion

        #region Методы для статистики

        public StudentStatistics GetStatistics()
        {
            var statistics = new StudentStatistics();
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                // Общее количество студентов
                string queryTotal = "SELECT COUNT(*) FROM Students";
                MySqlCommand cmdTotal = new MySqlCommand(queryTotal, connection);
                statistics.TotalStudents = Convert.ToInt32(cmdTotal.ExecuteScalar());

                // Студенты на бюджете
                string queryBudget = "SELECT COUNT(*) FROM Students WHERE isBudget = 1";
                MySqlCommand cmdBudget = new MySqlCommand(queryBudget, connection);
                statistics.BudgetStudents = Convert.ToInt32(cmdBudget.ExecuteScalar());

                // Студенты на контракте
                statistics.ContractStudents = statistics.TotalStudents - statistics.BudgetStudents;

                // Мужчины и женщины
                string queryMale = "SELECT COUNT(*) FROM Students WHERE Sex = 'М'";
                MySqlCommand cmdMale = new MySqlCommand(queryMale, connection);
                statistics.MaleStudents = Convert.ToInt32(cmdMale.ExecuteScalar());
                statistics.FemaleStudents = statistics.TotalStudents - statistics.MaleStudents;

                // Студенты по годам
                string queryYears = @"
                    SELECT YearReceipts, COUNT(*) as Count
                    FROM Students
                    GROUP BY YearReceipts
                    ORDER BY YearReceipts DESC";

                MySqlCommand cmdYears = new MySqlCommand(queryYears, connection);
                MySqlDataReader yearsData = cmdYears.ExecuteReader();
                while (yearsData.Read())
                {
                    int year = yearsData.GetInt32("YearReceipts");
                    int count = yearsData.GetInt32("Count");
                    statistics.StudentsByYear[year] = count;
                }
                yearsData.Close();

                // Студенты по отделениям
                string queryDept = @"
                    SELECT s.DepartmentId, d.Name, COUNT(*) as Count
                    FROM Students s
                    LEFT JOIN Departments d ON s.DepartmentId = d.Id
                    GROUP BY s.DepartmentId, d.Name
                    ORDER BY Count DESC";

                MySqlCommand cmdDept = new MySqlCommand(queryDept, connection);
                MySqlDataReader deptData = cmdDept.ExecuteReader();
                while (deptData.Read())
                {
                    int deptId = deptData.GetInt32("DepartmentId");
                    int count = deptData.GetInt32("Count");
                    statistics.StudentsByDepartment[deptId] = count;
                }
                deptData.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения статистики: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            return statistics;
        }

        #endregion

        #region Методы для экспорта данных

        public string ExportToCSV(List<StudentContext> students)
        {
            try
            {
                string csv = "Фамилия;Имя;Отчество;Дата рождения;Пол;Телефон;Образование;Группа;Тип финансирования;Год поступления;Год окончания;Отделение\n";

                foreach (var student in students)
                {
                    string financeType = student.isBudget == 1 ? "Бюджет" : "Контракт";
                    string deptName = GetDepartmentName(student.DepartmentId);

                    csv += $"\"{student.Surname}\";" +
                           $"\"{student.Firstname}\";" +
                           $"\"{student.Patronomyc}\";" +
                           $"\"{student.BirthDate:dd.MM.yyyy}\";" +
                           $"\"{student.Sex}\";" +
                           $"\"{student.Phone}\";" +
                           $"\"{student.Education}\";" +
                           $"\"{student.GroupName}\";" +
                           $"\"{financeType}\";" +
                           $"\"{student.YearReceipts.Year}\";" +
                           $"\"{student.YearFinish.Year}\";" +
                           $"\"{deptName}\"\n";
                }

                return csv;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка экспорта в CSV: {ex.Message}");
            }
        }

        private string GetDepartmentName(int departmentId)
        {
            try
            {
                var departmentContext = new DepartmentContext();
                var department = departmentContext.GetById(departmentId);
                return department?.Name ?? "Не указано";
            }
            catch
            {
                return "Не указано";
            }
        }

        #endregion
    }
}