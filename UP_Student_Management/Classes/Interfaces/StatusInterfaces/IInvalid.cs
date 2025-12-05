using System.Collections.Generic;
using UP_Student_Management.Classes.Context.StatusContext;

namespace UP_Student_Management.Classes.Interfaces.StatusInterfaces
{
    public interface IInvalid
    {
        void Save(bool Update = false);
        List<InvalidContext> AllInvalid();
        void Delete();
    }

}
