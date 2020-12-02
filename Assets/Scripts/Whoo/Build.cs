using System;
using AppLayer.NetworkGroups;
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

        private StartScreen startScreen;

        private RoomScreen roomScreen;

        private WaitingLobbyScreen waitingScreen;

        private FadeOut fadeOut;

        private static Build        Instance = null;
        public static  WhooSettings Settings { get; set; } = null;
        private static IScreen      _activeScreen;

        public void Awake()
        {
            Instance = this;
            Settings = settings;

            _activeScreen = null;
            startScreen   = Instantiate(settings.startScreen,   transform);
            roomScreen    = Instantiate(settings.roomScreen,    transform);
            waitingScreen = Instantiate(settings.waitingScreen, transform);
            fadeOut       = Instantiate(settings.fadeOut,       transform);

            startScreen.Hide();
            roomScreen.Hide();
            waitingScreen.Hide();
            fadeOut.Hide();

            ToStartScreen();
        }

        #region UI Transitions

        public static void ToStartScreen()
        {
            var instance = Instance;
            _activeScreen?.Close();
            instance.startScreen.Setup();
            instance.startScreen.Display();
            _activeScreen = instance.startScreen;

            var fadeSettings = Settings.DefaultFadeSettings;
            instance.fadeOut.Setup(ref fadeSettings);
            instance.fadeOut.Display();
            
            Fade();
        }

        public static void ToRoomScreen(StrapiRoom strapiRoom, INetworkGroup networkGroup, bool didCreate)
        {
            var instance = Instance;
            _activeScreen?.Close();

            var roomSettings = new RoomScreen.RoomSettings()
            {
                Room       = strapiRoom,
                LobbyGroup = networkGroup,
                DidCreate  = didCreate
            };
            instance.roomScreen.Setup(ref roomSettings);
            instance.roomScreen.Display();
            _activeScreen = instance.roomScreen;
        }

        private static void Fade()
        {
            var instance     = Instance;
            var fadeSettings = Settings.DefaultFadeSettings;
            instance.fadeOut.Setup(ref fadeSettings);
            instance.fadeOut.Display();
        }

        public static void RefreshRoomScreen()
        {
            Instance.roomScreen.Refresh();
        }

        #endregion

        public static void ToWaitingLobby(StrapiRoom room)
        {
            _activeScreen?.Close();

            var lobbySettings = new WaitingLobbyScreen.Settings()
            {
                Room = room
            };
            var instance = Instance;
            instance.waitingScreen.Setup(ref lobbySettings);
            instance.waitingScreen.Display();
            _activeScreen = instance.waitingScreen;
            
            Fade();
        }
    }
}