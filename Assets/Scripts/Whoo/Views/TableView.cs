using UnityEngine;
using UnityEngine.EventSystems;

namespace Whoo.Views
{
    public class TableView : GroupView
    {
        public RectTransform root;
        public RectTransform tableArea;

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
        }

        #region IPointerDownHandler
        
        protected override void MiddleClicked(PointerEventData eventData)
        {
            //ignore
        }

        protected override void RightClicked(PointerEventData eventData)
        {
            //ignore
        }

        protected override void LeftClicked(PointerEventData eventData)
        {
            if (Table.Group.LocalUser.IsSitting()) return;
            Table.SeatUserHere();
        }

        #endregion
    }
}