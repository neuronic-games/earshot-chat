using System.Collections.Generic;
using AppLayer.NetworkGroups;
using DiscordAppLayer;
using UI;
using UI.Screens;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Whoo.Views;

namespace Whoo
{
    public class RoomScreen : Screen<RoomScreen.RoomSettings>
    {
        #region Serialized

        [SerializeField]
        private GroupView room = null;

        private List<TableView> tables = new List<TableView>();

        [Header("Room Screen Properties")]
        public Button leaveRoomButton = null;

        public Button voiceSettingsButton = null;

        public CopiableText lobbyIdDisplay     = null;
        public CopiableText lobbySecretDisplay = null;

        [Header("Tables")]
        public RectTransform tableArea;

        #endregion

        #region Screen<RoomSettings>

        public struct RoomSettings : IScreenSettings
        {
            public INetworkGroup LobbyGroup;
        }

        public override void Setup()
        {
            Hide();

            leaveRoomButton.onClick.AddListener(LeaveRoom);
            voiceSettingsButton.onClick.AddListener(OpenVoiceSettings);
        }

        public override void Display(ref RoomSettings settings)
        {
            base.Display(ref settings);

            room.Setup(settings.LobbyGroup);

            LoadRoomTables(settings.LobbyGroup);
            
            Refresh();

            DiscordNetworkGroup lobby = settings.LobbyGroup as DiscordNetworkGroup;
            Assert.IsNotNull(lobby);

            lobbySecretDisplay.Text = lobby.Secret;
            lobbyIdDisplay.Text     = lobby.LobbyId.ToString();
        }

        private void LoadRoomTables(INetworkGroup group)
        {
        }

        public void AddDefaultTables()
        {
            var defaultTables = Build.Settings.defaultTablesWhenCreate;
            for (var i = 0; i < defaultTables.Length; i++)
            {
                CreateGroupAndTable(defaultTables[i]);
            }
        }

        #endregion

        #region UI Methods

        private bool _loading = false;

        public void LeaveRoom()
        {
            if (_loading) return;
            _loading = true;

            var lobby = room.Group as DiscordNetworkGroup;
            Assert.IsNotNull(lobby);

            if (lobby.IsConnectedVoice) lobby.DisconnectVoice();
            room.Group.LeaveOrDestroy((success) =>
            {
                _loading = false;
                if (success)
                {
                    room.Clear();
                    Build.ToStartScreen();
                }
            });
            foreach (var groupView in tables)
            {
                groupView.Clear();
                groupView.Group.LeaveOrDestroy(null);
                Destroy(groupView.gameObject);
            }
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

        #endregion

        /// <summary>
        /// Basically an Update method.
        /// </summary>
        public override void Refresh()
        {
            if (!room.Group.IsAlive)
            {
                Build.ToStartScreen();
                return;
            }

            room.ForceRefresh();
            for (var i = 0; i < tables.Count; i++)
            {
                var table = tables[i];
                if (!table.Group.IsAlive)
                {
                    Debug.Log($"Removing table\n {table.Group.ToString()}");
                    Destroy(table.gameObject);
                }
            }
        }

        public void CreateGroupAndTable(TableDisplayProperties props, bool retry = false)
        {
            var app = AppLayer.AppLayer.Get();
            app.CreateNewGroup(200, false, group =>
            {
                if (group == null)
                {
                    if(!retry)
                        Utils.DelayedAction(() => CreateGroupAndTable(props, true), 5.0f);
                    return;
                }
                var table = CreateTableFromGroup(group);
                table.SetTableSizes(props.rect, props.seats);
            });
        }

        public TableView CreateTableFromGroup(INetworkGroup group)
        {
            var groupView = tables.Find(t => t.Group == @group);
            if (groupView != null) return groupView;

            var table = Instantiate(Build.Settings.tableView, tableArea);
            table.Setup(group);
            return table;
        }
    }
}