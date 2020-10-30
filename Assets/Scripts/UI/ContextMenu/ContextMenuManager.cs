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
        public ClicksCatcher clicksCatcher;

        [Header("Templates")]
        public GameObject titleObject;

        public TextMeshProUGUI titleText;

        public MenuAction actionTemplate;
        public GameObject dividerTemplate;

        private Animator         menuAnimator;
        private List<MenuAction> _actionUis  = new List<MenuAction>();
        private List<GameObject> _dividerUis = new List<GameObject>();

        public void Start()
        {
            if (mainCanvas == null)
                mainCanvas = gameObject.GetComponentInParent<Canvas>();

            if (clicksCatcher == null)
                clicksCatcher = GetComponentInChildren<ClicksCatcher>();

            clicksCatcher.catchLeft   = true;
            clicksCatcher.catchMiddle = true;
            clicksCatcher.catchRight  = true;
            clicksCatcher.onCatch.AddListener(HideMenu);

            menuAnimator = contextMenuRect.parent.GetComponent<Animator>();
            if(menuAnimator == null)
                menuAnimator = contextMenuRect.parent.GetComponentInChildren<Animator>();
        }

        public void DisplayMenu(ContextMenu menu)
        {
            menuAnimator.Play("In");

            int actions  = 0;
            int dividers = 0;
            for (int i = menu.Entries.Count - 1; i >= 0; i--)
            {
                var entry = menu.Entries[i];
                if (entry.EntryType == ContextMenu.EntryType.Action)
                {
                    while (actions >= _actionUis.Count) _actionUis.Add(Instantiate(actionTemplate, contentContainer));
                    var actionUi = _actionUis[actions];
                    actionUi.gameObject.SetActive(true);
                    actionUi.transform.SetAsFirstSibling();
                    actionUi.Setup(this, entry);
                    actions++;
                }
                else if (entry.EntryType == ContextMenu.EntryType.Divider)
                {
                    while (dividers >= _dividerUis.Count) _dividerUis.Add(Instantiate(dividerTemplate, contentContainer));
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