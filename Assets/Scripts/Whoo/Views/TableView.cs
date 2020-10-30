using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Whoo.Views
{
    public class TableView : GroupView
    {
        public RectTransform root;
        public RectTransform tableArea;

        public UnityEvent onPropertiesUpdate;

        public Table Table { get; protected set; }

        public void Setup(Table table)
        {
            Table = table;
            base.Setup(table.Group);
            ResizeTable(Table.Properties.rect, Table.Properties.seats);
        }

        #region Table Sizing

        private void ResizeTable(Rect rect, int numSeats)
        {
            root.anchorMin        = Vector2.zero;
            root.anchorMax        = Vector2.zero;
            root.anchoredPosition = rect.position;
            tableArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.width);
            tableArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   rect.height);
        }

        #endregion

        protected override void OnGroupPropertiesUpdated()
        {
            ResizeTable(Table.Properties.rect, Table.Properties.seats);
            
            onPropertiesUpdate.Invoke();
        }

        #region IPointerDownHandler

        public override void MiddleClick()
        {
            //ignore
        }

        public override void RightClick()
        {
            //ignore
        }

        public override void LeftClick()
        {
            if (Table.Group.LocalUser.IsSitting()) return;
            Table.SeatUserHere();
        }

        #endregion
    }
}