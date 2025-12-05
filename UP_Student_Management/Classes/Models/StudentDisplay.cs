using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP_Student_Management.Classes.Models
{
    public class StudentDisplay
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string GroupName { get; set; }
        public string DepartmentName { get; set; }
        public string Phone { get; set; }
        public string Financing { get; set; }
        public int YearReceipts { get; set; }
        public int YearFinish { get; set; }
        public int DepartmentId { get; set; }
        public int isBudget { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string Patronomyc { get; set; }
        public DateTime BirthDate { get; set; }
        public string Sex { get; set; }
        public string Education { get; set; }
        public string Status { get; set; }
    }
}
