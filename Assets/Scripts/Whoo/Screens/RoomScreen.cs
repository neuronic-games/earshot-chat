using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
using DiscordAppLayer;
using UI;
using UI.Screens;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Whoo.Data;
using Whoo.Views;

namespace Whoo.Screens
{
    public class RoomScreen : Screen<RoomScreen.RoomSettings>
    {
        #region Serialized

        [SerializeField]
        private RoomView roomView = null;

        [Header("Room Screen Properties")]
        [SerializeField]
        private Button leaveRoomButton = null;

        [FormerlySerializedAs("voiceSettingsButton")]
        [SerializeField]
        private Button settingsButton = null;

        [SerializeField]
        private CopiableText roomId = null;

        [Header("Tables")]
        [SerializeField]
        private RawImage roomBackground = null;

        [SerializeField]
        private RectTransform tableArea = null;

        [Header("Settings")]
        [SerializeField]
        private SettingsScreen settingsScreen;

        #endregion

        #region Properties/Fields

        public WhooRoom WhooRoom { get; protected set; }

        private Dictionary<WhooTable, TableView> _tableViews = new Dictionary<WhooTable, TableView>();
        private HashSet<WhooTable>               _toRemove   = new HashSet<WhooTable>();

        #endregion

        #region Screen<RoomSettings>

        public struct RoomSettings : IScreenSettings
        {
            public StrapiRoom    Room;
            public INetworkGroup LobbyGroup;
            public bool          DidCreate;
        }

        public override async UniTask Setup(RoomSettings settings)
        {
            ResetWhooRoom();

            await base.Setup(settings);
            Assert.IsNotNull(settings.Room);
            Assert.IsNotNull(settings.LobbyGroup);
            WhooRoom = new WhooRoom(settings.Room, settings.LobbyGroup);

            WhooRoom.PropertyChanged += OnWhooRoomChanged;
            UpdateViews();
            await LoadBackgroundImageAsync(WhooRoom.StrapiRoom?.RoomModel?.layout);
            roomView.Setup(WhooRoom);

            roomId.Text = currentSettings.Room.RoomModel?.id;
        }

        public override async UniTask Close()
        {
            await Hide();

            ResetWhooRoom();

            Build.ToStartScreen().Forget();
        }

        /// <summary>
        /// Basically an Update method.
        /// </summary>
        public override async UniTask Refresh()
        {
            if (WhooRoom == null || !WhooRoom.RoomGroup.IsAlive)
            {
                await Close();
            }
        }

        #endregion

        #region Other Methods / Listeners

        private void OnWhooRoomChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is WhooRoom room)) return;
            roomView.Refresh();
            if (e.PropertyName == nameof(WhooRoom.Tables))
            {
                UpdateViews();
            }
            else if (e.PropertyName == nameof(WhooRoom.StrapiRoom))
            {
                LoadBackgroundImageAsync(room.StrapiRoom?.RoomModel?.layout).Forget();
            }
        }

        private async UniTask LoadBackgroundImageAsync(Layout layout)
        {
            Texture bg = await Utils.LoadPossibleWhooImage(layout?.image.FirstOrDefault()?.url);
            if (bg == null) throw new ExternalException("Layout does not supply an image.");
            //todo -- provide default image or throw user out of room?

            Rect  area       = tableArea.rect;
            float areaAspect = area.width / area.height;
            float bgAspect   = bg.width   / (float) bg.height;

            if (true || (bgAspect > areaAspect && bg.width  > area.width) ||
                (bgAspect         < areaAspect && bg.height > area.height))
            {
                tableArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bg.width);
                tableArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   bg.height);
                roomBackground.texture = bg;
            }
            else
            {
                roomBackground.ApplyTextureAndFit(bg);
            }
        }

        public void ResetWhooRoom()
        {
            if (WhooRoom != null)
            {
                var lobby = WhooRoom.RoomGroup as DiscordNetworkGroup;
                if (lobby != null)
                {
                    if (lobby.IsConnectedVoice) lobby.DisconnectVoice();
                }

                WhooRoom.PropertyChanged -= OnWhooRoomChanged;
                WhooRoom.Dispose();
                WhooRoom = null;
            }
        }

        public void UpdateViews()
        {
            for (var i = 0; i < WhooRoom.Tables.Count; i++)
            {
                WhooTable table = WhooRoom.Tables[i];
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
                if (kvp.Value == null || kvp.Key?.ZoneInstance == null)
                {
                    _toRemove.Add(kvp.Key);
                    kvp.Value.Clear();
                }
            }

            foreach (var table in _toRemove)
            {
                _tableViews.Remove(table);
            }
        }

        private void OpenSettingsPanel()
        {
            settingsScreen.Display();
        }

        #endregion

        #region Unity callbacks

        public void Awake()
        {
            leaveRoomButton.onClick.AddListener(ResetWhooRoom);
            settingsButton.onClick.AddListener(OpenSettingsPanel);
        }

        #endregion
    }
}