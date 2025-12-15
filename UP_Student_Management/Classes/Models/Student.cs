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
        public DateTime BirthDate { get; set; }
        public string Patronomyc { get; set; }
        public string Sex { get; set; }
        public string Phone { get; set; }
        public string Education { get; set; }
        public string GroupName { get; set; }
        public int isBudget { get; set; }
        public DateTime YearReceipts { get; set; }
        public DateTime YearFinish { get; set; }
        public string DeductionsInfo {  get; set; }// инфа об отчислении
        public DateTime DataDeductions { get; set; }
        public string Note {  get; set; }
        public string FilePath { get; set; }
        public int DepartmentId { get; set; }
        public int? RoomId { get; set; }
        public string ParentsInfo { get; set; }
        public string Penalties { get; set; }
    }
}
