using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using UP_Student_Management.Classes.Common;
using UP_Student_Management.Classes.Interfaces;
using UP_Student_Management.Classes.Models;

namespace UP_Student_Management.Classes.Context
{
    public class StudentFileContext : StudentFile, IStudentFile
    {
        // Реализация интерфейса IStudentFile

        #region Основные CRUD операции

        public StudentFileContext GetById(int id)
        {
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT Id, FilePath, IdStudent, CreatedDate
                FROM StudentFiles
                WHERE Id = @id";

            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@id", id);

                MySqlDataReader data = command.ExecuteReader();

                if (data.Read())
                {
                    return new StudentFileContext
                    {
                        Id = data.GetInt32("Id"),
                        FilePath = data.GetString("FilePath"),
                        IdStudent = data.GetInt32("IdStudent"),
                        CreatedDate = data.GetDateTime("CreatedDate")
                    };
                }

                data.Close();
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения файла: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public List<StudentFileContext> GetAllByStudentId(int studentId)
        {
            return GetFilesByStudentId(studentId);
        }

        public void Create()
        {
            AddFile();
        }

        public void Update()
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = @"
                    UPDATE StudentFiles 
                    SET FilePath = @filePath,
                        IdStudent = @idStudent
                    WHERE Id = @id";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@filePath", this.FilePath);
                command.Parameters.AddWithValue("@idStudent", this.IdStudent);
                command.Parameters.AddWithValue("@id", this.Id);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка обновления файла: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public void Delete()
        {
            DeleteFile();
        }

        #endregion

        #region Массовые операции

        public void DeleteAllForStudent(int studentId)
        {
            DeleteAllFilesByStudentId(studentId);
        }

        public int CountForStudent(int studentId)
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "SELECT COUNT(*) FROM StudentFiles WHERE IdStudent = @studentId";
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@studentId", studentId);

                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка подсчета файлов: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        #endregion

        #region Поиск

        public List<StudentFileContext> Search(string fileName, int? studentId = null)
        {
            List<StudentFileContext> files = new List<StudentFileContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT Id, FilePath, IdStudent, CreatedDate
                FROM StudentFiles
                WHERE FilePath LIKE @fileName";

            if (studentId.HasValue)
            {
                query += " AND IdStudent = @studentId";
            }

            query += " ORDER BY CreatedDate DESC";

            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@fileName", $"%{fileName}%");

                if (studentId.HasValue)
                {
                    command.Parameters.AddWithValue("@studentId", studentId.Value);
                }

                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    files.Add(new StudentFileContext
                    {
                        Id = data.GetInt32("Id"),
                        FilePath = data.GetString("FilePath"),
                        IdStudent = data.GetInt32("IdStudent"),
                        CreatedDate = data.GetDateTime("CreatedDate")
                    });
                }

                data.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка поиска файлов: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            return files;
        }

        public List<StudentFileContext> GetRecentFiles(int studentId, int days = 30)
        {
            List<StudentFileContext> files = new List<StudentFileContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT Id, FilePath, IdStudent, CreatedDate
                FROM StudentFiles
                WHERE IdStudent = @studentId 
                AND CreatedDate >= DATE_SUB(NOW(), INTERVAL @days DAY)
                ORDER BY CreatedDate DESC";

            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@studentId", studentId);
                command.Parameters.AddWithValue("@days", days);

                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    files.Add(new StudentFileContext
                    {
                        Id = data.GetInt32("Id"),
                        FilePath = data.GetString("FilePath"),
                        IdStudent = data.GetInt32("IdStudent"),
                        CreatedDate = data.GetDateTime("CreatedDate")
                    });
                }

                data.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения недавних файлов: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            return files;
        }

        #endregion

        #region Проверки

        public bool Exists(int id)
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "SELECT COUNT(*) FROM StudentFiles WHERE Id = @id";
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

        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        #endregion

        #region Информация о файлах

        public string GetFilePath(int id)
        {
            var file = GetById(id);
            return file?.FilePath;
        }

        public DateTime GetFileCreatedDate(int id)
        {
            var file = GetById(id);
            return file?.CreatedDate ?? DateTime.MinValue;
        }

        public string GetFileExtension(int id)
        {
            var filePath = GetFilePath(id);
            return !string.IsNullOrEmpty(filePath) ? Path.GetExtension(filePath) : string.Empty;
        }

        #endregion

        #region Статистика

        public int GetTotalFileCount()
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = "SELECT COUNT(*) FROM StudentFiles";
                MySqlCommand command = new MySqlCommand(query, connection);

                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения общего количества файлов: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }
        }

        public Dictionary<string, int> GetFileExtensionsStatistics(int studentId)
        {
            var statistics = new Dictionary<string, int>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    LOWER(SUBSTRING_INDEX(FilePath, '.', -1)) as Extension,
                    COUNT(*) as Count
                FROM StudentFiles
                WHERE IdStudent = @studentId
                GROUP BY LOWER(SUBSTRING_INDEX(FilePath, '.', -1))
                ORDER BY Count DESC";

            try
            {
                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@studentId", studentId);

                MySqlDataReader data = command.ExecuteReader();

                while (data.Read())
                {
                    string extension = data.GetString("Extension");
                    int count = data.GetInt32("Count");
                    statistics[extension] = count;
                }

                data.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Ошибка получения статистики расширений файлов: {ex.Message}");
            }
            finally
            {
                connection.Close();
            }

            return statistics;
        }

        #endregion

        #region Методы из оригинального класса (оставляем для обратной совместимости)

        public List<StudentFileContext> GetFilesByStudentId(int studentId)
        {
            List<StudentFileContext> files = new List<StudentFileContext>();
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
                    files.Add(new StudentFileContext
                    {
                        Id = data.GetInt32("Id"),
                        FilePath = data.GetString("FilePath"),
                        IdStudent = data.GetInt32("IdStudent"),
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

        public void AddFile()
        {
            MySqlConnection connection = Connection.OpenConnection();

            try
            {
                string query = @"
                    INSERT INTO StudentFiles (FilePath, IdStudent)
                    VALUES (@filePath, @idStudent)";

                MySqlCommand command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@filePath", this.FilePath);
                command.Parameters.AddWithValue("@idStudent", this.IdStudent);

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

        #endregion
    }
}