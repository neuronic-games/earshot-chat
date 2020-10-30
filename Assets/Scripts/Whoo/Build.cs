using AppLayer.NetworkGroups;
using Dialogs;
using DiscordAppLayer;
using TMPro;
using UI;
using UnityEngine;
using FadeOut = UI.Screens.FadeOut;

namespace Whoo
{
    public class Build : MonoBehaviour
    {
        public WhooSettings settings;

        private StartScreen startScreen;

        private RoomScreen roomScreen;

        private FadeOut fadeOut;

        private static Build        Instance = null;
        public static  WhooSettings Settings { get; private set; } = null;

        public void Awake()
        {
            Instance = this;
            Settings = settings;

            startScreen = Instantiate(settings.startScreen, transform);
            roomScreen  = Instantiate(settings.roomScreen,  transform);
            fadeOut     = Instantiate(settings.fadeOut,     transform);

            startScreen.Hide();
            roomScreen.Hide();
            fadeOut.Hide();

            ToStartScreen();
        }

        #region UI Transitions

        public static void ToStartScreen()
        {
            Instance.roomScreen.Close();
            Instance.startScreen.Setup();
            Instance.startScreen.Display();

            var fadeSettings = Settings.DefaultFadeSettings;
            Instance.fadeOut.Setup(ref fadeSettings);
            Instance.fadeOut.Display();
        }

        public static void ToRoomScreen(INetworkGroup networkGroup, bool didCreate)
        {
            Instance.startScreen.Hide();

            var roomSettings = new RoomScreen.RoomSettings() {LobbyGroup = networkGroup, DidCreate = didCreate};
            Instance.roomScreen.Setup(ref roomSettings);
            Instance.roomScreen.Display();

            var fadeSettings = Settings.DefaultFadeSettings;
            Instance.fadeOut.Setup(ref fadeSettings);
            Instance.fadeOut.Display();
        }

        public static void RefreshRoomScreen()
        {
            Instance.roomScreen.Refresh();
        }

        #endregion
    }
}