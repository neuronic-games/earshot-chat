using System;
using System.Collections.Generic;
using AppLayer.NetworkGroups;
using DiscordAppLayer;
using TMPro;
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
        private RoomView roomView = null;

        //private List<TableView> tables = new List<TableView>();
        [Header("Room Screen Properties")]
        public Button leaveRoomButton = null;

        public Button voiceSettingsButton = null;

        public CopiableText lobbyIdDisplay     = null;
        public CopiableText lobbySecretDisplay = null;

        [Header("Tables")]
        public RectTransform tableArea = null;

        public TextMeshProUGUI tablesLeftText = null;

        #endregion

        #region Screen<RoomSettings>

        public struct RoomSettings : IScreenSettings
        {
            public INetworkGroup LobbyGroup;
            public bool          DidCreate;
        }

        public Room Room { get; protected set; }

        private int _initialLoadingTables = 0;

        public override void Setup(ref RoomSettings settings)
        {
            base.Setup(ref settings);

            Room?.Dispose();
            Room = new Room(settings.LobbyGroup);
            
            roomView.Setup(Room);

            if (settings.DidCreate)
            {
                var defaultTables = Build.Settings.defaultTablesWhenCreate;
                for (int i = 0; i < defaultTables.Length; i++)
                {
                    int iUnchanging = i;
                    _initialLoadingTables++;
                    Utils.DelayedAction(() => Room.AddTable(defaultTables[iUnchanging], AddTable),
                        (iUnchanging + 1) * Constants.GroupCreateRate);

                    void AddTable(Table table)
                    {
                        _initialLoadingTables--;
                        if (table != null) return;
                        _initialLoadingTables++;
                        Utils.DelayedAction(() => Room.AddTable(defaultTables[iUnchanging], AddTable),
                            (iUnchanging + 1) * Constants.GroupCreateRate);
                    }
                }
            }

            DiscordNetworkGroup lobby = settings.LobbyGroup as DiscordNetworkGroup;
            Assert.IsNotNull(lobby);

            lobbySecretDisplay.Text = lobby.Secret;
            lobbyIdDisplay.Text     = lobby.LobbyId.ToString();
        }

        public override void Close()
        {
            Hide();
            if(Room != null && Room.RoomGroup.IsAlive)
            {
                LeaveRoom();
            }
        }

        /// <summary>
        /// Basically an Update method.
        /// </summary>
        public override void Refresh()
        {
            if (Room == null)
            {
                Hide();
            }
            else if (!Room.RoomGroup.IsAlive)
            {
                Close();
            }
            else
            {
                int expecting = _initialLoadingTables;
                if (expecting == 0) expecting = Room.ExpectedTables;
                tablesLeftText.text = expecting > 0 ? $"Loading Tables: {expecting}" : string.Empty;
                UpdateViews();
            }
            
        }

        #endregion

        #region UI Methods

        private bool _loading = false;

        public void LeaveRoom()
        {
            if (_loading) return;
            _loading = true;

            var lobby = Room.RoomGroup as DiscordNetworkGroup;
            Assert.IsNotNull(lobby);

            if (lobby.IsConnectedVoice) lobby.DisconnectVoice();

            _loading = false;
            Room.Dispose();
            Room = null;
            Build.ToStartScreen();
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


        #region Unity callbacks

        public void Awake()
        {
            leaveRoomButton.onClick.AddListener(LeaveRoom);
            voiceSettingsButton.onClick.AddListener(OpenVoiceSettings);
        }

        public void Update()
        {
            Refresh();
        }

        #endregion
        
        private Dictionary<Table, TableView> _tableViews = new Dictionary<Table, TableView>();
        private HashSet<Table> _toRemove = new HashSet<Table>();

        public void UpdateViews()
        {
            for (var i = 0; i < Room.Tables.Count; i++)
            {
                Table table = Room.Tables[i];
                if (!_tableViews.TryGetValue(table, out TableView tableView))
                {
                    tableView = Instantiate(Build.Settings.tableView, tableArea);
                    tableView.Setup(table);
                    _tableViews.Add(table, tableView);
                }
                tableView.Refresh();
            }
            
            _toRemove.Clear();
            foreach (var kvp in _tableViews)
            {
                if (kvp.Value == null || !kvp.Key.Group.IsAlive)
                {
                    _toRemove.Add(kvp.Key);
                    kvp.Value.Clear();
                }
            }
            foreach (var table in _toRemove)
            {
                _tableViews.Remove(table);
            }

            roomView.Refresh();
        }

        /*public void CreateGroupAndTable(TableDisplayProperties props, bool retry = false)
        {
            var app = AppLayer.AppLayer.Get();
            app.CreateNewGroup(200, false, group =>
            {
                if (group == null)
                {
                    if (!retry)
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
        }*/
    }
}