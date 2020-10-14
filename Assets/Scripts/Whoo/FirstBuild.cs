using AppLayer.NetworkGroups;
using DiscordAppLayer;
using UnityEngine;
using UnityEngine.UI;

namespace Whoo
{
    public class FirstBuild : MonoBehaviour
    {
        public GameObject startScreen;
        public GameObject roomScreen;

        public Button makeRoomButton;
        public Button leaveRoomButton;

        public Button voiceSettingsButton;

        public void Awake()
        {
            ToStartScreen();
            makeRoomButton.onClick.AddListener(MakeRoom);
            leaveRoomButton.onClick.AddListener(LeaveRoom);
            
            voiceSettingsButton.onClick.AddListener(OpenVoiceSettings);
        }

        private bool _loading    = false;
        private bool isRoomOwner = false;

        private INetworkGroup room = null;
        
        public void MakeRoom()
        {
            if (_loading) return;
            var app = AppLayer.AppLayer.Get();
            if (app.CanCreateGroup)
            {
                _loading    = true;
                isRoomOwner = true;
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

        /*
        public void JoinOrMakeRoom()
        {
            if (_loading) return;
            _loading = true;
            startScreen.SetActive(false);
            roomScreen.SetActive(true);
        }*/

        public void LeaveRoom()
        {
            if (_loading) return;
            _loading    = true;
            isRoomOwner = false;
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
                manager.OpenVoiceSettings(_ => {});
            }
            else
            {
                Debug.Log($"Discord app not found.");
            }
        }

        private void ToRoomScreen()
        {
            startScreen.SetActive(false);
            roomScreen.SetActive(true);
        }

        private void ToStartScreen()
        {
            startScreen.SetActive(true);
            roomScreen.SetActive(false);
        }
    }
}