using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Whoo.Data
{
    public interface IDataProvider
    {
        Data.RoomModel RoomModel { get; }
        List<Zone>     Zones     { get; }
        bool           Ready     { get; }
        UniTask        LoadRoom(string roomId);
        UniTask        Refresh();
    }
}