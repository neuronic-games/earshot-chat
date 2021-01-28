using UnityEngine;
using UnityEngine.UI;

namespace UI.FlowerMenu
{
    public class FlowerMenuLayout : LayoutGroup
    {
        [Header("Sizing")]
        public float itemSize;
        public float radius;
        [Header("Positioning")]
        public int   totalItems;
        public float angle;

        public override void CalculateLayoutInputVertical()
        {
            childAlignment = TextAnchor.MiddleCenter;

            int childCount = 0;
            for (int i = 0; i < rectTransform.childCount; i++)
            {
                if (rectTransform.GetChild(i).gameObject.activeSelf) childCount++;
            }

            Vector2 center = rectTransform.rect.center;
            bool    even   = childCount % 2 == 0;

            rectTransform.sizeDelta = Vector2.one * itemSize;


            for (int i = 0; i < rectChildren.Count; i++)
            {
                var item = rectChildren[i];

                if (!item.gameObject.activeSelf) continue;

                var     factor = (even ? (Mathf.Floor(i / 2.0f) + 0.5f) : (Mathf.Ceil(i / 2.0f))) * Mathf.Pow(-1, i);
                float   finalAngle = (angle + factor * (360f / totalItems)) * Mathf.Deg2Rad;
                Vector2 pos = new Vector2(Mathf.Cos(finalAngle), Mathf.Sin(finalAngle)).normalized * radius;

                SetChildAlongAxis(item, 0, center.x + pos.x, itemSize);
                SetChildAlongAxis(item, 1, center.y + pos.y, itemSize);
            }
        }

        public override void SetLayoutHorizontal()
        {
        }

        public override void SetLayoutVertical()
        {
        }

        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();
        }
    }
}