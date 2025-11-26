using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UP_Student_Management.Classes.Models
{
    public class StudentCategory
    {
        public int Id { get; set; }
        public string Name { get; set; } // "Сирота", "Инвалид", "ОВЗ", "Общежитие", "СВО", "Социальная стипендия", "СОП/Группа риска"
    }
}
