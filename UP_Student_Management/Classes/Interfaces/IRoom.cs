using System.Collections.Generic;
using UP_Student_Management.Classes.Context;

namespace UP_Student_Management.Classes.Interfaces
{
    public interface IRoom
    {
        void Save(bool Update = false);
        List<RoomContext> AllRooms();
        void Delete();
    }
}
