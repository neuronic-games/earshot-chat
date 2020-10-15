using AppLayer.NetworkGroups;
using Dialogs;
using DiscordAppLayer;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Whoo
{
    public class FirstBuild : MonoBehaviour
    {
        [Header("Screens")]
        public GameObject startScreen;

        public Room roomScreen;

        public Button makeRoomButton;

        [Header("Room Screen Properties")]
        public Button leaveRoomButton;

        public Button voiceSettingsButton;

        public CopiableText lobbyIdDisplay;
        public CopiableText lobbySecretDisplay;

        [Header("Joining")]
        public Button joinButton;

        public TMP_InputField lobbyIdInput;
        public TMP_InputField lobbySecretInput;

        public void Awake()
        {
            ToStartScreen();
            makeRoomButton.onClick.AddListener(MakeRoom);
            leaveRoomButton.onClick.AddListener(LeaveRoom);

            voiceSettingsButton.onClick.AddListener(OpenVoiceSettings);

            joinButton.onClick.AddListener(JoinRoom);
        }

        private bool _loading = false;

        private INetworkGroup room = null;

        public void MakeRoom()
        {
            if (_loading) return;
            var app = AppLayer.AppLayer.Get();
            if (app.CanCreateGroup)
            {
                _loading = true;
                app.CreateNewGroup(5, false, group =>
                {
                    _loading = false;
                    room     = group;
                    if (group == null) return;
                    ToRoomScreen();
                });
            }
            else
            {
                Debug.Log($"Can't create any more groups.");
            }
        }


        public void JoinRoom()
        {
            if (_loading) return;
            _loading = true;
            if (!long.TryParse(lobbyIdInput.text, out long lobbyId))
            {
                Dialog.Get().RequestInfo("Invalid Input", "Lobby Id is incorrect.", DialogStyle.Error, null);
                return;
            }

            string secret = lobbySecretInput.text.Trim(' ');

            var app = AppLayer.AppLayer.Get();
            if (app.CanJoinGroup)
            {
                app.JoinGroup(lobbyId, secret, group =>
                {
                    _loading = false;
                    room     = group;
                    if (group == null) return; //should be logged by logging service
                    ToRoomScreen();
                });
            }
            else
            {
                Debug.Log($"Can't join any more groups.");
            }
        }

        public void LeaveRoom()
        {
            if (_loading) return;
            _loading = true;
            var lobby = room as DiscordNetworkGroup;
            Assert.IsNotNull(lobby);
            if(lobby.IsConnectedVoice) lobby.DisconnectVoice();
            room.LeaveOrDestroy((success) =>
            {
                _loading = false;
                if (success)
                {
                    room = null;
                    ToStartScreen();
                }
            });
        }

        private void OpenVoiceSettings()
        {
            if (DiscordApp.GetDiscordApp(out var discord))
            {
                var manager = discord.OverlayManager;
                manager.OpenVoiceSettings(_ => { });
            }
            else
            {
                Debug.Log($"Discord app not found.");
            }
        }

        public void ToRoomScreen()
        {
            startScreen.SetActive(false);
            roomScreen.gameObject.SetActive(true);
            roomScreen.Setup(room);
            DiscordNetworkGroup lobby = room as DiscordNetworkGroup;
            Assert.IsNotNull(lobby);
            lobbySecretDisplay.Text = lobby.Secret;
            lobbyIdDisplay.Text     = lobby.LobbyId.ToString();

            if (!lobby.IsConnectedVoice)
            {
                lobby.ConnectVoice(null, null);
                lobby.OnDisconnectVoice += VoiceDisconnected;
            }
        }

        private void VoiceDisconnected()
        {
            if (room is DiscordNetworkGroup lobby) lobby.OnDisconnectVoice -= VoiceDisconnected;
            
            Debug.Log($"Voice disconnected.");
        }

        public void ToStartScreen()
        {
            startScreen.SetActive(true);
            roomScreen.gameObject.SetActive(false);
        }
    }
}