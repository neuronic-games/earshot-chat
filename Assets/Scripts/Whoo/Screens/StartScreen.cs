using System;
using System.Diagnostics;
using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
using MLAPI;
using MLAPI.Configuration;
using Networking;
using TMPro;
using UI.Dialogs;
using UnityEngine;
using UnityEngine.UI;
using Whoo.Data;
using Debug = UnityEngine.Debug;
using Screen = UI.Screens.Screen;

namespace Whoo.Screens
{
    public class StartScreen : Screen
    {
        #region Serializable

        [Header("Joining")] public Button joinButton;

        public LayoutSelectorScreen layoutSelector;

        public RoomSelectorScreen roomSelector;

        public Button makeRoomButton;

        public Button yourRooms;

        public TMP_InputField joinRoomInput;

        #endregion

        public void Awake()
        {
            yourRooms.onClick.AddListener(() => DisplayUserRoomsAsync().Forget());

            joinButton.onClick.AddListener(JoinRoom);
            makeRoomButton.onClick.AddListener(MakeRoom);
        }


        #region Screen

        public override async UniTask Setup()
        {
            _loading = false;
            joinRoomInput.text = string.Empty;
            await layoutSelector.Hide();
            await roomSelector.Hide();
        }

        #endregion

        private bool _loading = false;

        #region UI Methods

        public async UniTaskVoid DisplayUserRoomsAsync()
        {
            if (_loading) return;
            var settings = new RoomSelectorScreen.Settings()
            {
                OnSelected = _MakeRoom,
                ProfileId = Build.Settings.testerInfo.profileId
            };
            await roomSelector.Setup(settings);
            await roomSelector.Display();

            async UniTaskVoid _MakeRoom(RoomModel roomModel)
            {
                _loading = true;
                await roomModel.EnsureHasZoneInstancedAsync();
                StrapiRoom room = new StrapiRoom();
                await room.LoadRoom(roomModel.id);
                JoinRoomAsync(room).Forget();
            }
        }

        public void MakeRoom()
        {
            


            if (_loading) return;
            var settings = new LayoutSelectorScreen.Settings() {OnSelected = _MakeRoom};
            layoutSelector.Setup(settings).Forget();
            layoutSelector.Display().Forget();

            async UniTaskVoid _MakeRoom(Layout layout)
            {
                _loading = true;
                NetLauncher.StartHost();
                StrapiRoom room = await StrapiRoom.CreateNew(layout, Build.Settings.testerInfo.profileId);
                JoinRoomAsync(room).Forget();
            }
        }

        /*private async UniTaskVoid ToRoomScreenAsync(StrapiRoom room, INetworkGroup group, bool didCreate)
        {
            var (id, secret) = group.IdAndPassword;

            var roomCredentials = room.RoomModel.room_credentials;
            if (roomCredentials?.platform_id != id || roomCredentials?.platform_secret != secret)
            {
                await room.RoomModel.PutAsync(new RoomModel()
                {
                    room_credentials = new PlatformCredentials()
                    {
                        platform        = PlatformCredentials.Platform_Discord,
                        platform_id     = id,
                        platform_secret = secret
                    }
                }, string.Empty, Utils.StrapiModelSerializationDefaults());
            }

            await room.RoomModel.EnsureHasZoneInstancedAsync();

            Build.ToRoomScreen(room, group, didCreate).Forget();
        }*/

        private async UniTaskVoid ToRoomScreenAsync(StrapiRoom room, bool didCreate, INetworkGroup group = null)
        {
            /*var (id, secret) = group.IdAndPassword;

            var roomCredentials = room.RoomModel.room_credentials;
            if (roomCredentials?.platform_id != id || roomCredentials?.platform_secret != secret)
            {
                await room.RoomModel.PutAsync(new RoomModel()
                {
                    room_credentials = new PlatformCredentials()
                    {
                        platform        = PlatformCredentials.Platform_Discord,
                        platform_id     = id,
                        platform_secret = secret
                    }
                }, string.Empty, Utils.StrapiModelSerializationDefaults());
            }*/

            await room.RoomModel.EnsureHasZoneInstancedAsync();

            Build.ToRoomScreen(room, group, didCreate).Forget();
        }

        private void JoinRoom()
        {
            NetLauncher.StartClient();
            JoinRoomAsync().Forget();
        }

        public async UniTaskVoid JoinRoomAsync()
        {
            //TODO
            if (_loading) return;

            // todo vz: consider to grab IP:port from url in the provided text field if the latter is provided
            string roomId = joinRoomInput.text;
            var roomModel = new RoomModel() {id = roomId};
            try
            {
                await (roomModel.GetAsync());
            }
            catch
            {
                //wrong room id
                Dialog.Get().RequestInfo("Unable To Join", "The room id entered is invalid. Please check it again.",
                    DialogStyle.Info, null);
                return;
            }
            
            StrapiRoom room = new StrapiRoom();
            await room.LoadRoom(roomModel.id);
            JoinRoomAsync(room).Forget();
        }

        public async UniTaskVoid JoinRoomAsync(StrapiRoom room)
        {
            _loading = false;

            // todo: do we need this INetworkGroup since a user (maybe) should be added to a group when a zone is selected
            //INetworkGroup group = await Utils.JoinGroup(room.RoomModel);

            // if (group == null)
            // {
            //     StrapiPlatformInfoInvalid().Forget();
            // }
            // else
            // {
            //successfully joined group
            ToRoomScreenAsync(room, false).Forget();
            //}

            /*async UniTaskVoid StrapiPlatformInfoInvalid()
            {
                var profileId = Build.Settings.testerInfo.profileId;
                if (room.RoomModel.owner != null && room.RoomModel.owner?.id == profileId)
                {
                    //we are the owner of this room
                    group = await Utils.CreateGroup((uint) room.RoomModel.layout.capacity);
                    if (group == null)
                    {
                        //unable to create group... todo -- keep trying
                        Debug.Log("Unable to create platform group.");
                        _loading = false;
                        return;
                    }

                    Debug.Log($"Created new group for strapi room.");

                    ToRoomScreenAsync(room, group, true).Forget();
                }
                else
                {*/
            //GoToWaitingLobby();
            //}
            //}

            void GoToWaitingLobby()
            {
                Build.ToWaitingLobby(room).Forget();
            }
        }

        #endregion
    }
}