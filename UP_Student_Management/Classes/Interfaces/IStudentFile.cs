using System;
using System.Collections.Generic;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Classes.Interfaces
{
    public interface IStudentFile
    {
        // Основные CRUD операции
        StudentFileContext GetById(int id);
        List<StudentFileContext> GetAllByStudentId(int studentId);
        void Create();
        void Update();
        void Delete();

        // Массовые операции
        void DeleteAllForStudent(int studentId);
        int CountForStudent(int studentId);

        // Поиск
        List<StudentFileContext> Search(string fileName, int? studentId = null);
        List<StudentFileContext> GetRecentFiles(int studentId, int days = 30);

        // Проверки
        bool Exists(int id);
        bool FileExists(string filePath);

        // Информация о файлах
        string GetFilePath(int id);
        DateTime GetFileCreatedDate(int id);
        string GetFileExtension(int id);

        // Статистика
        int GetTotalFileCount();
        Dictionary<string, int> GetFileExtensionsStatistics(int studentId);
    }
}