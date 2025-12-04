using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP_Student_Management.Classes.Models.StatusModels
{
    public class SPPP
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string CallReason { get; set; }
        public string EmployeesPresent { get; set; }
        public string RepresentativesPresent { get; set; }
        public string ReasonCall { get; set; }
        public string Decision { get; set; }
        public string Note { get; set; }
        public string DocumentPath { get; set; }
        public DateTime Date { get; set; }
    }
}
