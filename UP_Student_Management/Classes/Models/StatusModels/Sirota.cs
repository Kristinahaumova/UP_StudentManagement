using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP_Student_Management.Classes.Models.StatusModels
{
    public class Sirota
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string Prikaz { get; set; }
        public string Note { get; set; }
        public string DocumentPath { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
