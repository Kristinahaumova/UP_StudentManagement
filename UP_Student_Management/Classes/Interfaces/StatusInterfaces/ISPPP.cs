using System.Collections.Generic;
using UP_Student_Management.Classes.Context.StatusContext;

namespace UP_Student_Management.Classes.Interfaces.StatusInterfaces
{
    public interface ISPPP
    {
        void Save(bool Update = false);
        List<SPPPContext> AllSPPP();
        void Delete();
    }
}
