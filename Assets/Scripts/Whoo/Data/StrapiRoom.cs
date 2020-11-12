using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Whoo.Data
{
    public class StrapiRoom : IDataProvider
    {
        #region IDataProvider

        public RoomData       RoomData  { get; private set; }
        public List<ZoneData> ZoneDatas { get; set; }
        public bool           Ready     { get; private set; }

        private string _roomId = string.Empty;

        public StrapiRoom()
        {
            RoomData  = default;
            ZoneDatas = new List<ZoneData>();
        }

        public async UniTask LoadRoom(string roomId)
        {
            ZoneDatas.Clear();
            _roomId = roomId;
            await Refresh();
        }

        public async UniTask Refresh()
        {
            await RefreshRoom();
            await RefreshZones();
        }

        //todo -- handle web request failure

        public async UniTask RefreshRoom()
        {
            Ready = false;

            RoomData = RoomData ?? new RoomData();

            await RoomData.Fill(_roomId);

            Ready = true;
        }

        public async UniTask RefreshZones()
        {
            Ready = false;

            ZoneDatas.Clear();
            List<UniTask> pendingOps = new List<UniTask>();

            var roomZones = RoomData.room_zones;
            for (var i = 0; i < roomZones.Count; i++)
            {
                while (i >= ZoneDatas.Count)
                {
                    ZoneDatas.Add(new ZoneData());
                }
                ZoneData zoneData = ZoneDatas[i];
                
                pendingOps.Add(zoneData.Fill(roomZones[i].zone));
            }
            
            while(roomZones.Count < ZoneDatas.Count)
                ZoneDatas.RemoveAt(ZoneDatas.Count - 1);

            await UniTask.WhenAll(pendingOps);

            Ready = true;
        }

        #endregion
    }
}