using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Manipulators
{
    public class UIClampedDraggable : UIDraggable
    {
        public RectTransform clampConstraint;

        private Vector3[] _targetCorners = new Vector3[4];
        private Vector3[] _clampCorners  = new Vector3[4];

        public override void OnDrag(PointerEventData eventData)
        {
            target.GetWorldCorners(_targetCorners);
            clampConstraint.GetWorldCorners(_clampCorners);

            Clamp(eventData.delta.x, ScrollType.Horizontal);
            Clamp(eventData.delta.y, ScrollType.Vertical);
        }

        private void Clamp(float dy, ScrollType scrollMode)
        {
            float targetMin = scrollMode == ScrollType.Horizontal ? _targetCorners[0].x : _targetCorners[0].y;
            float targetMax = scrollMode == ScrollType.Horizontal ? _targetCorners[2].x : _targetCorners[2].y;
            float clampMin  = scrollMode == ScrollType.Horizontal ? _clampCorners[0].x : _clampCorners[0].y;
            float clampMax  = scrollMode == ScrollType.Horizontal ? _clampCorners[2].x : _clampCorners[2].y;

            float finalOffset = 0;
            //we can scroll in given direction if either there is ample space, or corner in other direction is hidden
            if (dy < 0 && (targetMin + dy > clampMin || (targetMax > clampMax)))
            {
                finalOffset = dy;
            }
            else if (dy > 0 && (targetMax + dy < clampMax || (targetMin < clampMin)))
            {
                finalOffset = dy;
            }

            //-ve scroll goes towards minimum
            switch (scrollMode)
            {
                case ScrollType.Horizontal:
                    target.anchoredPosition += new Vector2(finalOffset, 0);
                    break;
                case ScrollType.Vertical:
                    target.anchoredPosition += new Vector2(0, finalOffset);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }
}