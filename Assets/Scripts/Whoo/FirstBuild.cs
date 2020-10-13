using AppLayer.NetworkGroups;
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

        public void Awake()
        {
            ToStartScreen();
            makeRoomButton.onClick.AddListener(MakeRoom);
            leaveRoomButton.onClick.AddListener(LeaveRoom);
        }

        private bool _loading    = false;
        private bool isRoomOwner = false;

        private INetworkGroup room = null;
        
        public void MakeRoom()
        {
            if (_loading) return;
            _loading    = true;
            isRoomOwner = true;
            var app = AppLayer.AppLayer.Get();
            if (app.CanCreateGroup)
            {
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
            var app = AppLayer.AppLayer.Get();
            app.DeleteGroup(room);
            room = null;
            ToStartScreen();
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