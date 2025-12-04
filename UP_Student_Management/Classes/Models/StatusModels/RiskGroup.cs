using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP_Student_Management.Classes.Models.StatusModels
{
    public class RiskGroup
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Type { get; set; }
        public string Note { get; set; }
        public string RegistrationOsnovanie { get; set; }
        public string RemovalOsnovanie { get; set; }
        public string RegistrationReason { get; set; }
        public string RemovalReason { get; set; }
        public string DocumentPath { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
