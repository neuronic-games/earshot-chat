using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Whoo.Data
{
    public interface IDataProvider
    {
        Layout       Layout  { get; }
        List<Zone> ZoneDatas { get; }
        bool           Ready     { get; }
        UniTask        LoadRoom(string roomId);
        UniTask        Refresh();
    }
}