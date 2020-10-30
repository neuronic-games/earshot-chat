using System;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// Only supports overlay camera.
    /// </summary>
    public class TooltipManager : Singleton<TooltipManager>
    {
        public Canvas        mainCanvas;
        public RectTransform tooltipRect;
        public RectTransform tooltipContent;

        [Header("Settings"), Range(0.0f, 0.5f)]
        public float pointerFollowSmoothness = 0.1f;

        public bool isUpdating = false;

        public float borderTop    = -15;
        public float borderBottom = 15;
        public float borderLeft   = 15;
        public float borderRight  = -15;

        private RectTransform tooltipZHelper;
        private Vector3       cursorPos;
        private Vector2       uiPos;
        private Vector3       tooltipVelocity;
        private Vector3       contentPos = Vector3.zero;

        public void Start()
        {
            if (mainCanvas == null)
                mainCanvas = gameObject.GetComponentInParent<Canvas>();

            if (tooltipRect == null)
                tooltipRect = gameObject.GetComponentInChildren<RectTransform>();

            if (tooltipContent == null)
                tooltipContent = gameObject.GetComponentInChildren<RectTransform>();

            if (tooltipText == null)
                tooltipText = gameObject.GetComponentInChildren<TextMeshProUGUI>();

            tooltipAnimator = tooltipRect.GetComponent<Animator>();

            tooltipZHelper = GetComponentInParent<RectTransform>();

            transform.SetAsLastSibling();
        }

        public void Update()
        {
            if (isUpdating)
            {
                cursorPos   = Input.mousePosition;
                cursorPos.z = tooltipZHelper.position.z;
                uiPos       = tooltipRect.anchoredPosition;
                CheckBounds();
                
                if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    tooltipRect.position = cursorPos;
                    tooltipContent.transform.position = Vector3.SmoothDamp(tooltipContent.transform.position, cursorPos + contentPos, ref tooltipVelocity, pointerFollowSmoothness);
                }
            }
        }

        public Vector4 boundsMax;

        private void CheckBounds()
        {
            if (uiPos.x <= boundsMax.x)
            {
                contentPos           = new Vector3(borderLeft, contentPos.y, 0);
                tooltipContent.pivot = new Vector2(0f, tooltipContent.pivot.y);
            }

            if (uiPos.x >= boundsMax.y)
            {
                contentPos           = new Vector3(borderRight, contentPos.y, 0);
                tooltipContent.pivot = new Vector2(1f, tooltipContent.pivot.y);
            }

            if (uiPos.y <= boundsMax.z)
            {
                contentPos           = new Vector3(contentPos.x, borderBottom, 0);
                tooltipContent.pivot = new Vector2(tooltipContent.pivot.x, 0f);
            }

            if (uiPos.y >= boundsMax.w)
            {
                contentPos           = new Vector3(contentPos.x, borderTop, 0);
                tooltipContent.pivot = new Vector2(tooltipContent.pivot.x, 1f);
            }
        }

        [Header("Contents")]
        public TextMeshProUGUI tooltipText;

        private Animator tooltipAnimator;

        public void SetContent(string text)
        {
            tooltipText.text = text;
            isUpdating = true;
            tooltipAnimator.Play("In");
        }

        public void UpdateContentDontAnimate(string text)
        {
            tooltipText.text = text;
            isUpdating = true;
            tooltipAnimator.Play("In", 0, 1.0f);
        }

        public void Hide()
        {
            isUpdating = false;
            tooltipAnimator.Play("Out");
        }
    }
}