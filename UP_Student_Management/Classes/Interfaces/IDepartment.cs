using System.Collections.Generic;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Classes.Interfaces
{
    public interface IDepartment
    {
        void Save(bool Update = false);
        List<DepartmentContext> AllDepartments();
        void Delete();
    }
}
