using UnityEngine;
using UnityEngine.EventSystems;

namespace Whoo.Views
{
    public class TableView : GroupView, IPointerDownHandler
    {
        public RectTransform root;
        public RectTransform tableArea;

        public override void ForceRefresh()
        {
            base.ForceRefresh();

            ResizeTable();
        }

        #region Table Sizing
        
        public void ResizeTable()
        {
            if (Group.CustomProperties.TryGetValue(Constants.DisplayProperties, out string fullString))
            {
                string[] props    = fullString.Split(',');
                float    posX     = float.Parse(props[0]);
                float    posY     = float.Parse(props[1]);
                float    width    = float.Parse(props[2]);
                float    height   = float.Parse(props[3]);
                int      numSeats = int.Parse(props[4]);

                ResizeTable(new Rect(posX, posY, width, height), numSeats);
            }
        }

        private void ResizeTable(Rect rect, int numSeats)
        {
            root.anchorMin        = Vector2.zero;
            root.anchorMax        = Vector2.zero;
            root.anchoredPosition = rect.position;
            tableArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rect.x);
            tableArea.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,   rect.y);
        }

        public void SetTableSizes(Rect rect, int numSeats)
        {
            Group.SetOrDeleteCustomProperty(Constants.DisplayProperties,
                $"{rect.x},{rect.y},{rect.width},{rect.height},{numSeats}");
            ResizeTable(rect, numSeats);
        }
        
        #endregion

        #region Events

        //todo -- more sophisticated event distribution

        public void OnPointerDown(PointerEventData eventData)
        {
            Build.MoveLocalUserToGroup(Group);
        }

        #endregion
    }
}