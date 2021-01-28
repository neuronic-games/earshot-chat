using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.ContextMenu
{
    [DefaultExecutionOrder(-100)]
    public class ContextMenuManager : Singleton<ContextMenuManager>
    {
        public Canvas        mainCanvas;
        public RectTransform contextMenuRect;
        public Transform     contentContainer;
        public LocalClicksCatcher clicksCatcher;

        [Header("Templates")]
        public GameObject titleObject;

        public TextMeshProUGUI titleText;

        public MenuAction actionTemplate;
        public GameObject dividerTemplate;

        [Header("Bounds")]
        public float borderTop = -15;

        public float   borderBottom = 15;
        public float   borderLeft   = 15;
        public float   borderRight  = -15;
        public Vector4 boundsMax;

        private Animator         menuAnimator;
        private RectTransform    zHelper;
        private RectTransform    contextMenuContent;
        private List<MenuAction> _actionUis  = new List<MenuAction>();
        private List<GameObject> _dividerUis = new List<GameObject>();

        public void Start()
        {
            if (mainCanvas == null)
                mainCanvas = gameObject.GetComponentInParent<Canvas>();

            if (clicksCatcher == null)
                clicksCatcher = GetComponentInChildren<LocalClicksCatcher>();

            zHelper            = GetComponentInParent<RectTransform>();
            contextMenuContent = contextMenuRect.GetComponentInChildren<RectTransform>();

            clicksCatcher.catchLeft   = true;
            clicksCatcher.catchMiddle = true;
            clicksCatcher.catchRight  = true;
            clicksCatcher.onCatch.AddListener(HideMenu);

            menuAnimator = contextMenuRect.parent.GetComponent<Animator>();
            if (menuAnimator == null)
                menuAnimator = contextMenuRect.parent.GetComponentInChildren<Animator>();
        }

        public void DisplayMenu(ContextMenu menu)
        {
            menuAnimator.Play("In");

            PopulateContextMenu();

            Vector3 cursorPos = Input.mousePosition;
            cursorPos.z = zHelper.position.z;

            Vector2 contentPosition = CheckBounds();

            SetPosition();

            void PopulateContextMenu()
            {
                int actions  = 0;
                int dividers = 0;
                for (int i = menu.Entries.Count - 1; i >= 0; i--)
                {
                    var entry = menu.Entries[i];
                    if (entry.EntryType == ContextMenu.EntryType.Action)
                    {
                        while (actions >= _actionUis.Count)
                            _actionUis.Add(Instantiate(actionTemplate, contentContainer));
                        var actionUi = _actionUis[actions];
                        actionUi.gameObject.SetActive(true);
                        actionUi.transform.SetAsFirstSibling();
                        actionUi.Setup(this, entry);
                        actions++;
                    }
                    else if (entry.EntryType == ContextMenu.EntryType.Divider)
                    {
                        while (dividers >= _dividerUis.Count)
                            _dividerUis.Add(Instantiate(dividerTemplate, contentContainer));
                        var dividerUi = _dividerUis[dividers];
                        dividerUi.gameObject.SetActive(true);
                        dividerUi.transform.SetAsFirstSibling();
                        dividers++;
                    }

                    titleObject.SetActive(menu.Title.EntryType == ContextMenu.EntryType.Title);
                    titleText.text = menu.Title.Name;
                    titleObject.transform.SetAsFirstSibling();
                }

                for (int i = actions; i < _actionUis.Count; i++)
                {
                    _actionUis[i].gameObject.SetActive(false);
                }

                for (int i = dividers; i < _dividerUis.Count; i++)
                {
                    _dividerUis[i].gameObject.SetActive(false);
                }
            }

            Vector2 CheckBounds()
            {
                Vector2 uiPos      = contextMenuRect.anchoredPosition;
                Vector2 contentPos = Vector2.zero;
                if (uiPos.x <= boundsMax.x)
                {
                    contentPos               = new Vector3(borderLeft, contentPos.y, 0);
                    contextMenuContent.pivot = new Vector2(0f, contextMenuContent.pivot.y);
                }

                if (uiPos.x >= boundsMax.y)
                {
                    contentPos               = new Vector3(borderRight, contentPos.y, 0);
                    contextMenuContent.pivot = new Vector2(1f, contextMenuContent.pivot.y);
                }

                if (uiPos.y <= boundsMax.z)
                {
                    contentPos               = new Vector3(contentPos.x, borderBottom, 0);
                    contextMenuContent.pivot = new Vector2(contextMenuContent.pivot.x, 0f);
                }

                if (uiPos.y >= boundsMax.w)
                {
                    contentPos               = new Vector3(contentPos.x, borderTop, 0);
                    contextMenuContent.pivot = new Vector2(contextMenuContent.pivot.x, 1f);
                }

                return contentPos;
            }

            void SetPosition()
            {
                if (mainCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    contextMenuRect.position = cursorPos;
                    contextMenuContent.transform.position =
                        cursorPos + new Vector3(contentPosition.x, contentPosition.y, 0);
                }
            }
        }

        public void HideMenu()
        {
            menuAnimator.Play("Out");
        }

        public void NotifyClicked(MenuAction menuAction)
        {
            HideMenu();
        }
    }
}