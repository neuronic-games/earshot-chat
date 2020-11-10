using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Whoo.Data
{
    public interface IDataProvider
    {
        RoomData       RoomData  { get; }
        List<ZoneData> ZoneDatas { get; }
        bool           Ready     { get; }
        UniTask        LoadRoom(string roomId);
        UniTask        Refresh();
    }
}