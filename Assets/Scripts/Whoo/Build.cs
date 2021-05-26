using System;
using System.Collections.Generic;
using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
using UI.Screens;
using UnityEngine;
using Whoo.Data;
using Whoo.Screens;
using FadeOut = UI.Screens.FadeOut;

namespace Whoo
{
    public class Build : MonoBehaviour
    {
        public WhooSettings settings;

        private AuthenticationScreen authScreen;

        private StartScreen startScreen;

        private RoomScreen roomScreen;

        private WaitingLobbyScreen waitingScreen;

        private GameObject fadeOut;

        private static Build                       Instance = null;
        public static  WhooSettings                Settings { get; set; } = null;
        public static  List<IAuthenticatedContext> AuthContexts = new List<IAuthenticatedContext>();

        private static IScreen _activeScreen;

        public void Awake()
        {
            Instance = this;
            Settings = settings;

            _activeScreen = null;
            authScreen    = Instantiate(settings.authScreen,    transform);
            startScreen   = Instantiate(settings.startScreen,   transform);
            roomScreen    = Instantiate(settings.roomScreen,    transform);
            waitingScreen = Instantiate(settings.waitingScreen, transform);
            fadeOut       = Instantiate(settings.fadeOut,       transform);

            authScreen.Hide().Forget();
            startScreen.Hide().Forget();
            roomScreen.Hide().Forget();
            waitingScreen.Hide().Forget();
            fadeOut.SetActive(false);

            ToStartScreen().Forget();
        }

        #region UI Transitions

        public static async UniTask ToStartScreen()
        {
            var instance = Instance;
            instance.fadeOut.SetActive(true);
            
            if(_activeScreen == instance.startScreen)
                return;

            if (_activeScreen != null)
                await _activeScreen.Close();

            try
            {
                if (await ContextIsValid())
                {
                    await instance.startScreen.Setup();
                    await instance.startScreen.Display();
                    _activeScreen = instance.startScreen;
                }
                else
                {
                    await instance.authScreen.Setup();
                    await instance.authScreen.Display();
                    _activeScreen = instance.authScreen;
                }
            }
            finally
            {
                instance.fadeOut.SetActive(false);
            }
        }

        public static async UniTask ToRoomScreen(StrapiRoom strapiRoom, INetworkGroup networkGroup, bool didCreate)
        {
            var instance = Instance;
            instance.fadeOut.SetActive(true);

            if (_activeScreen != null)
                await _activeScreen.Close();

            try
            {
                var roomSettings = new RoomScreen.RoomSettings()
                {
                    Room       = strapiRoom,
                    LobbyGroup = networkGroup,
                    DidCreate  = didCreate
                };
                await instance.roomScreen.Setup(roomSettings);
                await instance.roomScreen.Display();
                _activeScreen = instance.roomScreen;
            }
            finally
            {
                instance.fadeOut.SetActive(false);
            }
        }

        public static void RefreshRoomScreen()
        {
            Instance.roomScreen.Refresh().Forget();
        }

        #endregion

        public static async UniTask ToWaitingLobby(StrapiRoom room)
        {
            var instance = Instance;
            _activeScreen?.Close();

            instance.fadeOut.SetActive(true);

            var lobbySettings = new WaitingLobbyScreen.Settings()
            {
                Room = room
            };
            await instance.waitingScreen.Setup(lobbySettings);
            await instance.waitingScreen.Display();
            _activeScreen = instance.waitingScreen;

            instance.fadeOut.SetActive(true);
        }

        public static async UniTask<bool> ContextIsValid()
        {
            var auth = AuthContexts.Find(ctx => ctx is StrapiAuthenticatedUser);
            return auth != null && await auth.IsValid();
        }
    }
}