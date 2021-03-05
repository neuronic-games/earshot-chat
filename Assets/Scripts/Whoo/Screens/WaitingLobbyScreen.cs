using System;
using System.Linq;
using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
using UI.Screens;
using UnityEngine;
using UnityEngine.UI;
using Whoo.Data;

namespace Whoo.Screens
{
    public class WaitingLobbyScreen : Screen<WaitingLobbyScreen.Settings>
    {
        [SerializeField]
        private Button backToStart = default;

        [SerializeField]
        private RawImage image = default;

        public void Awake()
        {
            backToStart.onClick.AddListener(BackToStartScreen);
        }

        private void BackToStartScreen()
        {
            CurrentSettings = default;
            Build.ToStartScreen().Forget();
        }

        #region Screen

        public struct Settings : IScreenSettings
        {
            public StrapiRoom Room;
        }

        public override async UniTask Setup(Settings settings)
        {
            await base.Setup(settings);
            //wow... this next statement
            Texture tex =
                await Utils.LoadPossibleWhooImage(CurrentSettings.Room?.RoomModel?.layout?.image.FirstOrDefault()?.url);
            image.ApplyTextureAndFit(tex);
        }

        public override async UniTask Refresh()
        {
            if (string.IsNullOrEmpty(CurrentSettings.Room?.RoomModel?.id))
            {
                BackToStartScreen();
                return;
            }

            await CurrentSettings.Room.RefreshRoom();
            INetworkGroup group = await Utils.JoinGroup(CurrentSettings.Room?.RoomModel);
            if (group == null) return;
            Build.ToRoomScreen(CurrentSettings.Room, group, false).Forget();
        }

        #endregion

        private float _lastRefresh = 0.0f;

        public void Update()
        {
            var refreshInterval = Whoo.Build.Settings.waitingLobbyRefreshInterval;
            if (Time.time > _lastRefresh + refreshInterval)
            {
                _lastRefresh = Time.time;
                Refresh().Forget();
            }
        }
    }
}