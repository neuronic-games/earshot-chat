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

        private async UniTask RefreshRoom()
        {
            Ready = false;
            var roomEndpoint = StrapiEndpoints.RoomEndpoint(_roomId);
            string response = (await UnityWebRequest.Get(roomEndpoint).SendWebRequest()).
                              downloadHandler.text;

            RoomData = new RoomData();
            JsonUtility.FromJsonOverwrite(response, RoomData);

            Ready = true;
        }

        private async UniTask RefreshZones()
        {
            Ready = false;

            List<UniTask<UnityWebRequest>> pendingOps = new List<UniTask<UnityWebRequest>>();

            for (var i = 0; i < RoomData.room_zones.Count; i++)
            {
                pendingOps.Add(UnityWebRequest.Get(StrapiEndpoints.RoomEndpoint(_roomId)).SendWebRequest().ToUniTask());
            }

            UnityWebRequest[] result = await UniTask.WhenAll(pendingOps);

            ZoneDatas.Clear();

            for (var i = 0; i < pendingOps.Count; i++)
            {
                var zoneData = new ZoneData();
                JsonUtility.FromJsonOverwrite(result[i].downloadHandler.text, zoneData);
                ZoneDatas.Add(zoneData);
            }

            Ready = true;
        }

        #endregion
    }
}