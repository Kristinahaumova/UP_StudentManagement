using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UP_Student_Management.Classes.Context.StatusContext;

namespace UP_Student_Management.Classes.Interfaces.StatusInterfaces
{
    public interface ISirota
    {
        void Save(bool Update = false);
        List<SirotaContext> AllSirota();
        void Delete();
    }

}
