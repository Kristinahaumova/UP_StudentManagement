using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace UP_Student_Management.Classes.Models
{
    public class StudentFile
    {
        public int Id { get; set; }
        public string FilePath { get; set; }
        public int IdStudent { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}