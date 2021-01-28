using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Cysharp.Threading.Tasks;
using UI;
using UI.FlowerMenu;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Whoo.Data;

namespace Whoo.Views
{
    public class TableView : GroupView
    {
        #region Serialized

        [Header("Positioning")]
        [SerializeField]
        private Vector2 anchorMin = new Vector2(0, 0);

        [SerializeField]
        private Vector2 anchorMax = new Vector2(0, 0);

        [SerializeField]
        private RectTransform root = default;

        [SerializeField]
        private RectTransform tableArea = default;

        [Header("Seating Arrangement")]
        [SerializeField]
        private SeatArrangement seating = default;

        [SerializeField]
        private MenuItem underSeatObject;

        [SerializeField]
        private RectTransform underSeatContainer;

        [Header("Other")]
        [SerializeField]
        private RawImage image = default;

        public UnityEvent onPropertiesUpdate = default;

        #endregion

        public WhooTable Table { get; protected set; }

        #region Table Binding

        public void Setup(WhooTable table)
        {
            Table = table;
            base.Setup(table.Group);

            SetupSeatingArrangement();

            Table.PropertyChanged += OnTableUpdated;

            ZoneInstanceUpdated();
        }

        #region Seating Management

        private void SetupSeatingArrangement()
        {
            seating.OnLayoutChanged += () => UpdateUnderSeatsAsync().Forget();
            var zone = Table.ZoneInstance.zone;
            seating.seats = Table.WhooRoom.StrapiRoom.Zones.Find(z => z.id == zone.id).seats;
            SetupUnderSeats(seating.seats);
        }

        private async UniTaskVoid UpdateUnderSeatsAsync()
        {
            await UniTask.DelayFrame(1);
            _underSeats.ForEach(u => u.gameObject.SetActive(true));
            for (int i = 0; i < seating.ChildrenCount && i < _underSeats.Count; i++)
            {
                _underSeats[i].gameObject.SetActive(false);
            }

            if (seating.seats.Count < _underSeats.Count)
            {
                for (int i = seating.seats.Count; i < _underSeats.Count; i++)
                {
                    _underSeats[i].gameObject.SetActive(false);
                }
            }
        }

        private List<MenuItem> _underSeats = new List<MenuItem>();

        protected void SetupUnderSeats(List<Zone.Seat> seats)
        {
            if (seats == null)
            {
                _underSeats.ForEach(u => u.gameObject.SetActive(false));
                return;
            }

            while (seats.Count > _underSeats.Count)
            {
                var under = Instantiate(underSeatObject, underSeatContainer);
                _underSeats.Add(under);
            }

            for (int i = 0; i < seats.Count && i < _underSeats.Count; i++)
            {
                var under = _underSeats[i];
                var seat  = seats[i];
                under.root.anchorMin        = seating.anchorMin;
                under.root.anchorMax        = seating.anchorMax;
                under.root.pivot            = seating.pivot;
                under.root.anchoredPosition = (Vector2) seat * seating.offsetModifier;
                under.button.onClick.RemoveAllListeners();
                under.button.onClick.AddListener(() => OnSeatSelected(seat));
            }

            _underSeats.ForEach(u => u.gameObject.SetActive(true));
        }

        #endregion

        private void OnSeatSelected(Zone.Seat seat)
        {
            //todo -- seat on specific seats
            SeatHere();
        }

        private void OnTableUpdated(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is WhooTable table)) return;
            switch (e.PropertyName)
            {
                case nameof(WhooTable.Group):
                    base.Setup(table.Group);
                    break;
                case nameof(WhooTable.ZoneInstance):
                    ZoneInstanceUpdated();
                    break;
            }
        }

        private void ZoneInstanceUpdated()
        {
            bool visibile = CheckTableVisibility();
            if (visibile)
            {
                ResizeTable();
                CheckBackgroundImageAsync(Table.ZoneInstance?.zone?.image?.FirstOrDefault()?.url).Forget();
            } //todo -- seats
        }

        private async UniTaskVoid CheckBackgroundImageAsync(string url)
        {
            var loadedImage =
                await Utils.LoadPossibleWhooImage(url, false, true, gameObject.GetCancellationTokenOnDestroy());
            image.texture =
                loadedImage == null ? Build.Settings.transparent.texture : loadedImage;
        }

        #endregion

        #region Table Manipulators

        private bool CheckTableVisibility()
        {
            Zone zone    = Table.ZoneInstance?.zone;
            var  visible = zone != null;
            gameObject.SetActive(visible);
            return visible;
        }

        private void ResizeTable()
        {
            Zone zone = Table.ZoneInstance?.zone;
            if (zone == null)
            {
                return;
            }

            ResizeTable(new Rect(zone.x, zone.y, zone.width, zone.height));
        }

        private void ResizeTable(Rect rect)
        {
            root.anchorMin        = anchorMin;
            root.anchorMax        = anchorMax;
            tableArea.anchorMin   = anchorMin;
            tableArea.anchorMax   = anchorMax;
            root.anchoredPosition = rect.position * new Vector2(1, -1);
            tableArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
            tableArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   rect.height);
        }

        #endregion

        #region Group View

        #region IUserListView

        public override string ListUniqueId => Table.ZoneInstance.zone.id;

        #endregion

        protected override void OnUsersUpdated()
        {
        }

        protected override void OnGroupPropertiesUpdated()
        {
            onPropertiesUpdate.Invoke();
        }

        public void SeatHere()
        {
            Table.SeatUserHere();
        }

        #endregion
    }
}