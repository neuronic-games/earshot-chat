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

            startScreen.Setup();
            roomScreen.Setup();
            fadeOut.Setup();

            ToStartScreen();
        }

        #region UI Transitions

        public static void ToStartScreen()
        {
            Instance.roomScreen.Hide();
            Instance.startScreen.Display(default);
            var fadeSettings = Settings.DefaultFadeSettings;
            Instance.fadeOut.Display(ref fadeSettings);
        }

        public static void ToRoomScreen(INetworkGroup networkGroup, bool didCreate)
        {
            Instance.startScreen.Hide();

            var roomSettings = new RoomScreen.RoomSettings() {LobbyGroup = networkGroup};
            Instance.roomScreen.Display(ref roomSettings);
            var fadeSettings = Settings.DefaultFadeSettings;
            Instance.fadeOut.Display(ref fadeSettings);

            if (DiscordApp.GetDiscordApp(out DiscordApp app))
            {
                var localUser = app.LocalDiscordUser;
                localUser.SetOrDeleteCustomProperty(Constants.Sitting, Constants.True,
                    networkGroup as DiscordNetworkGroup);
            }

            if (didCreate)
            {
                Instance.roomScreen.AddDefaultTables();
            }
        }

        public static void RefreshRoomScreen()
        {
            Instance.roomScreen.Refresh();
        }

        #endregion

        #region Application Actions

        public static void MoveUserToGroup(IUser user, INetworkGroup move)
        {
            if (user.Group == move) return;
            user.SetOrDeleteCustomProperty(Constants.Sitting, null);
            var members = move.Members;
            for (var i = 0; i < members.Count; i++)
            {
                var otherMember = members[i];
                if (otherMember.Equals(user))
                {
                    otherMember.SetOrDeleteCustomProperty(Constants.Sitting, Constants.True);
                    if (user.Group is DiscordNetworkGroup discordGroup && discordGroup.IsConnectedVoice)
                    {
                        discordGroup.DisconnectVoice();
                    }

                    if (move is DiscordNetworkGroup discordGroup1)
                    {
                        discordGroup1.ConnectVoice(null, null);
                    }

                    return;
                }
            }

            //not connected to other group, shouldn't be possible
        }

        public static void MoveLocalUserToGroup(INetworkGroup toGroup)
        {
            IUser local = AppLayer.AppLayer.Get().LocalUser;
            MoveUserToGroup(local, toGroup);
        }

        #endregion
    }
}