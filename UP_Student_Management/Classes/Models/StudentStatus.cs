using System;

namespace UP_Student_Management.Classes.Models
{
    public class StudentStatus
    {
        public string StatusType { get; set; }
        public string Note { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
