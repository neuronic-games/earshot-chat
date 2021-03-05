using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Whoo.Data
{
    public class StrapiRoom : IDataProvider
    {
        #region IDataProvider

        public RoomModel      RoomModel { get; private set; }
        public List<Zone>     Zones     { get; set; }
        public bool           Ready     { get; private set; }

        private string _roomId = string.Empty;

        public event Action<RoomModel>      OnRoomRefreshed;
        public event Action<List<Zone>>     OnZonesRefreshed;

        public StrapiRoom()
        {
            RoomModel = default;
            Zones     = new List<Zone>();
        }

        public async UniTask LoadRoom(string roomId)
        {
            _roomId = roomId;
            await Refresh();
        }

        public async UniTask Refresh()
        {
            await RefreshRoom();
            await RefreshZones();
            //await RefreshOccupants();
        }

        //todo -- handle web request failure

        public async UniTask RefreshRoom()
        {
            Ready = false;

            RoomModel = RoomModel ?? new RoomModel() {id = _roomId};

            await RoomModel.GetAsync();

            OnRoomRefreshed?.Invoke(RoomModel);

            Ready = true;
        }

        public async UniTask RefreshZones()
        {
            if (RoomModel.layout == null)
            {
                return; //todo
            }

            Ready = false;

            Zones = await RoomModel.layout.GetAllZonesAsync();

            OnZonesRefreshed?.Invoke(Zones);

            Ready = true;
        }

        #endregion

        public static async UniTask<StrapiRoom> CreateNew(Layout layout, string profileId)
        {
            RoomModel model = new RoomModel();
            await model.PostAsync(new
            {
                layout = layout.id,
                owner  = profileId,
                name   = "My Room"
            }, string.Empty, Utils.StrapiModelSerializationDefaults());
            await model.EnsureHasZoneInstancedAsync();
            StrapiRoom room = new StrapiRoom();
            await room.LoadRoom(model.id);
            return room;
        }
    }
}