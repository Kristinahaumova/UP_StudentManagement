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
        public List<StudentContext> AllStudents()
        {
            List<StudentContext> allStudents = new List<StudentContext>();
            MySqlConnection connection = Connection.OpenConnection();

            string query = @"
                SELECT 
                    Id, Firstname, Surname, BirthDate, Patronomyc, 
                    Sex, Phone, Education, GroupName, isBudget,
                    YearReceipts, YearFinish, DeductionsInfo, 
                    DataDeductions, Note, FilePath, DepartmentId
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

                if (!data.IsDBNull(data.GetOrdinal("FilePath")))
                    student.FilePath = data.GetString("FilePath");

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
                            FilePath = @filePath,
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
                         DeductionsInfo, DataDeductions, Note, FilePath, DepartmentId) 
                        VALUES 
                        (@firstname, @surname, @birthdate, @patronomyc, @sex, @phone,
                         @education, @groupName, @isBudget, @yearReceipts, @yearFinish,
                         @deductionsInfo, @dataDeductions, @note, @filePath, @departmentId)";

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
            command.Parameters.AddWithValue("@filePath", this.FilePath ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@departmentId", this.DepartmentId);
        }

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

            return student;
        }

    }
}