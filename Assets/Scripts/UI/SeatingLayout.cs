using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SeatingLayout : LayoutGroup
    {
        public bool    fitSeats;
        public Vector2 seatSize;
        public Vector2 spacing;

        public Order order;
        public Edges startEdge;

        public enum Order
        {
            Clockwise,
            Anticlockwise
        }

        public enum Edges
        {
            Left,
            Right,
            Bottom,
            Top
        }

        public override void CalculateLayoutInputVertical()
        {
            int childCount = 0;
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                if (rectTransform.GetChild(i).gameObject.activeSelf) childCount++;
            }


            Edges currentEdge = startEdge;

            int placedSeat = 0;
            int aroundTable = 0;
            for (int i = 0; i < rectChildren.Count; i++)
            {
                var item = rectChildren[i];

                if (!item.gameObject.activeSelf) continue;
                
                Rect parentRect = rectTransform.rect;

                float cellWidth  = seatSize.x;
                float cellHeight = seatSize.y;

                if (!fitSeats)
                {
                    var rect = item.rect;
                    cellWidth  = rect.width;
                    cellHeight = rect.height;
                }

                float spacingX, spacingY, xpos, ypos;
                
                CalculateItemPosition(parentRect,aroundTable, cellWidth, cellHeight, out xpos, out ypos, out spacingX, out spacingY);

                while(true)
                {
                    if (currentEdge == Edges.Bottom || currentEdge == Edges.Top)
                    {
                        float totalWidth   = parentRect.width;
                        float widthCovered = (cellWidth + Mathf.Abs(spacingX)) * (placedSeat + 1);
                        if (widthCovered > totalWidth)
                        {
                            currentEdge = NextEdge(currentEdge, order);
                            placedSeat  = 0;
                            aroundTable++;
                            CalculateItemPosition(parentRect, aroundTable, cellWidth, cellHeight, out xpos, out ypos, out spacingX,
                                out spacingY);
                            continue;
                        }
                    }
                    else if (currentEdge == Edges.Left || currentEdge == Edges.Right)
                    {
                        float totalHeight   = parentRect.height;
                        float heightCovered = (cellHeight + Mathf.Abs(spacingY)) * (placedSeat + 1);
                        if (heightCovered > totalHeight)
                        {
                            currentEdge = NextEdge(currentEdge, order);
                            placedSeat  = 0;
                            aroundTable++;
                            CalculateItemPosition(parentRect, aroundTable, cellWidth, cellHeight, out xpos, out ypos, out spacingX,
                                out spacingY);
                            continue;
                        }
                    }

                    break;
                }
                SetChildAlongAxis(item, 0, xpos + spacingX, cellWidth);
                SetChildAlongAxis(item, 1, ypos + spacingY, cellHeight);

                placedSeat++;
            }

            void CalculateItemPosition(Rect parentRect, int aroundRect, float cellWidth, float cellHeight, out float xpos, out float ypos, out float spacingX, out float spacingY)
            {
                Vector2 offset = (seatSize + spacing) * (aroundTable / 4);
                xpos = GetHorizontalAlongEdge(parentRect, order, currentEdge, cellWidth, spacing, placedSeat, out spacingX);
                ypos = GetVerticalAlongEdge(parentRect, order, currentEdge, cellHeight, spacing, placedSeat, out spacingY);
                switch (currentEdge)
                {
                    case Edges.Left:
                        xpos -= offset.x;
                        break;
                    case Edges.Right: 
                        xpos += offset.x;
                        break;
                    case Edges.Bottom: 
                        ypos += offset.y;
                        break;
                    case Edges.Top:
                        ypos -= offset.y;
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
        }

        private Edges NextEdge(Edges currentEdge, Order placeOrder)
        {
            switch (currentEdge)
            {
                case Edges.Left: return placeOrder == Order.Clockwise ? Edges.Top : Edges.Bottom;
                case Edges.Right: return placeOrder == Order.Clockwise ? Edges.Bottom : Edges.Top;
                case Edges.Bottom: return placeOrder == Order.Clockwise ? Edges.Left : Edges.Right;
                case Edges.Top: return placeOrder == Order.Clockwise ? Edges.Right : Edges.Left;
                default: throw new ArgumentOutOfRangeException(nameof(currentEdge), currentEdge, null);
            }
        }

        private float GetHorizontalAlongEdge(Rect parentRect, Order placeOrder, Edges currentEdge, float width, Vector2 spacing, int i, out float spacingX)
        {
            switch (currentEdge)
            {
                case Edges.Left:
                    spacingX = - spacing.y;
                    return 0 - width;
                case Edges.Right:
                    spacingX = spacing.y;
                    return parentRect.width;
                case Edges.Bottom:
                    if (placeOrder == Order.Anticlockwise)
                    {
                        spacingX = spacing.x;
                        return (width + spacingX) * i;
                    }
                    else
                    {
                        spacingX = - spacing.x;
                        return parentRect.width - (width - spacingX) * (i) - width;
                    }
                case Edges.Top: 
                    if (placeOrder == Order.Clockwise)
                    {
                        spacingX = spacing.x;
                        return (width + spacingX) * i;
                    }
                    else
                    {
                        spacingX = - spacing.x;
                        return parentRect.width - (width - spacingX) * i - width;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(currentEdge), currentEdge, null);
            }
        }
        
        private float GetVerticalAlongEdge(Rect parentRect, Order placeOrder, Edges currentEdge, float height, Vector2 spacing, int i, out float spacingY)
        {
            switch (currentEdge)
            {
                case Edges.Bottom:
                    spacingY = spacing.y;
                    return parentRect.height;
                case Edges.Top:
                    spacingY = - spacing.y;
                    return 0 - height;
                case Edges.Right: 
                    if (placeOrder == Order.Clockwise)
                    {
                        spacingY = spacing.x;
                        return (height + spacingY) * i;
                    }
                    else
                    {
                        spacingY = - spacing.x;
                        return parentRect.height - (height - spacingY) * i - height;
                    }
                case Edges.Left: 
                    if (placeOrder == Order.Anticlockwise)
                    {
                        spacingY = spacing.x;
                        return (height + spacingY) * i;
                    }
                    else
                    {
                        spacingY = - spacing.x;
                        return parentRect.height - (height - spacingY) * i - height;
                    }
                default: throw new ArgumentOutOfRangeException(nameof(currentEdge), currentEdge, null);
            }
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }
    }
}