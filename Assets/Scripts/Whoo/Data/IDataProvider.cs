using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Whoo.Data
{
    public interface IDataProvider
    {
        Data.RoomModel           RoomModel      { get; }
        List<Zone>     Zones { get; }
        List<Occupant> Occupants { get; }
        bool           Ready     { get; }
        UniTask        LoadRoom(string roomId);
        UniTask        Refresh();
    }
}