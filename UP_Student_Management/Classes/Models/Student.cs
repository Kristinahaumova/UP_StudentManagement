using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP_Student_Management.Classes.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Patronomyc { get; set; }
        public DateTime BirthDate { get; set; }
        public int DepartmentId { get; set; }
        public Department Department { get; set; }
        public int? RoomId { get; set; }
        public Room Room { get; set; }
        public List<StudentCategoryLink> StudentCategories { get; set; }
    }
}
