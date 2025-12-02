using System.Collections.Generic;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Classes.Interfaces
{
    public interface IStudent
    {
        // Основные CRUD операции
        List<StudentContext> AllStudents();
        void Save(bool Update = false);
        void Delete();
    }
}