using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Whoo.Data;

namespace Whoo.Views
{
    public class SeatArrangement : LayoutGroup
    {
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 pivot;
        public Vector2 offsetModifier;

        public int ChildrenCount { get; private set; }

        public event Action OnLayoutChanged;

        public List<Zone.Seat> seats = default;

        public override void CalculateLayoutInputVertical()
        {
            ChildrenCount = rectChildren.Count;
            for (int i = 0; i < rectChildren.Count; i++)
            {
                RectTransform child = rectChildren[i];
                if (i >= seats.Count)
                {
                    continue;
                }

                child.anchorMin        = anchorMin;
                child.anchorMax        = anchorMax;
                child.pivot            = pivot;
                child.anchoredPosition = (Vector2) seats[i] * offsetModifier;
            }

            ChildrenCount = rectChildren.Count;
            OnLayoutChanged?.Invoke();
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }
    }
}