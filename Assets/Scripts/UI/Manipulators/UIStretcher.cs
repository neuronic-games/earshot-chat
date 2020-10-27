using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Manipulators
{
    public enum StretchType
    {
        Left,
        Right,
        Top,
        Bottom
    }

    public class UIStretcher : UIManipulator, IDragHandler
    {
        public StretchType stretchType;
        public Vector2     minimumSize;

        private Vector3[] _corners = new Vector3[4];

        public void OnDrag(PointerEventData eventData)
        {
            target.GetLocalCorners(_corners);

            Vector2 bottomleft = _corners[0];
            Vector2 topright   = _corners[2];

            float width  = topright.x - bottomleft.x;
            float height = topright.y - bottomleft.y;

            float dx = eventData.delta.x;
            float dy = eventData.delta.y;

            switch (stretchType)
            {
                case StretchType.Left:
                    if (dx > 0 && width - dx < minimumSize.x) break;
                    target.offsetMin += new Vector2(eventData.delta.x, 0);
                    break;
                case StretchType.Right:
                    if (dx < 0 && width + dx < minimumSize.x) break;
                    target.offsetMax += new Vector2(eventData.delta.x, 0);
                    break;
                case StretchType.Top:
                    if (dy < 0 && height + dy < minimumSize.y) break;
                    target.offsetMax += new Vector2(0, eventData.delta.y);
                    break;
                case StretchType.Bottom:
                    if (dy > 0 && height - dy < minimumSize.y) break;
                    target.offsetMin += new Vector2(0, eventData.delta.y);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}