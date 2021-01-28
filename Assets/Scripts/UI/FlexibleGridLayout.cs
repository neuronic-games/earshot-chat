using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            Uniform,
            Width,
            Height,
            FixedRows,
            FixedColumns
        }

        public int     rows;
        public int     columns;
        public Vector2 cellSize;
        public Vector2 spacing;
        public FitType fitType;

        public bool fitX;
        public bool fitY;

        public override void CalculateLayoutInputVertical()
        {
            int childCount = 0;
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                if (rectTransform.GetChild(i).gameObject.activeSelf) childCount++;
            }
            if (fitType == FitType.Height || fitType == FitType.Width || fitType == FitType.Uniform)
            {
                fitX = true;
                fitY = true;
                
                float sqrt = Mathf.Sqrt(childCount);
                rows    = Mathf.CeilToInt(sqrt);
                columns = Mathf.CeilToInt(sqrt);
            }

            if (fitType == FitType.Height || fitType == FitType.FixedRows)
            {
                columns = Mathf.CeilToInt(childCount / (float) rows);
            }
            else if (fitType == FitType.Width || fitType == FitType.FixedColumns)
            {
                rows = Mathf.CeilToInt(childCount / (float) columns);
            }

            float parentWidth  = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            var padding1 = padding;
            float cellWidth = (parentWidth   / (float) columns) - ((spacing.x / (float) columns) * (columns - 1)) -
                              (padding1.left / (float) columns) - (padding1.right                / (float) columns);
            float cellHeight = (parentHeight / (float) rows) - ((spacing.y / (float) rows) * (rows - 1)) -
                               (padding1.top / (float) rows) - (padding1.bottom            / (float) rows);

            cellSize.x = fitX ? cellWidth : cellSize.x;
            cellSize.y = fitY ? cellHeight : cellSize.y;

            int columnCount = 0;
            int rowCount    = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                rowCount    = i / columns;
                columnCount = i % columns;

                var item = rectChildren[i];

                if (!item.gameObject.activeSelf) continue;

                var xpos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding1.left;
                var ypos = (cellSize.y * rowCount) + (spacing.y * rowCount)       + padding1.top;

                SetChildAlongAxis(item, 0, xpos, cellSize.x);
                SetChildAlongAxis(item, 1, ypos, cellSize.y);
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