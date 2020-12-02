﻿using System;
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
            currentSettings = default;
            Build.ToStartScreen();
        }

        #region Screen

        public struct Settings : IScreenSettings
        {
            public StrapiRoom Room;
        }

        public override void Setup(ref Settings settings)
        {
            base.Setup(ref settings);
            SetupAsync().Forget();
        }

        public async UniTaskVoid SetupAsync()
        {
            //wow... this next statement
            Texture tex =
                await Utils.LoadPossibleWhooImage(currentSettings.Room?.RoomModel?.layout?.image.FirstOrDefault()?.url);
            image.ApplyTextureAndFit(tex);
        }

        public override void Refresh()
        {
            RefreshAsync().Forget();
        }

        public async UniTaskVoid RefreshAsync()
        {
            if (string.IsNullOrEmpty(currentSettings.Room?.RoomModel?.id))
            {
                BackToStartScreen();
                return;
            }

            await currentSettings.Room.RefreshRoom();
            INetworkGroup group = await Utils.JoinGroup(currentSettings.Room?.RoomModel);
            if (group == null) return;
            Build.ToRoomScreen(currentSettings.Room, group, false);
        }

        #endregion

        private float _lastRefresh = 0.0f;

        public void Update()
        {
            var refreshInterval = Whoo.Build.Settings.waitingLobbyRefreshInterval;
            if (Time.time > _lastRefresh + refreshInterval)
            {
                _lastRefresh = Time.time;
                Refresh();
            }
        }
    }
}