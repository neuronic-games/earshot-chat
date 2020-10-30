using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Whoo;

namespace UI.ContextMenu
{
    public abstract class ContextMenuArea : MonoBehaviour, IPointerDownHandler
    {
        [Header("Config")]
        public bool openLeftClick;

        public bool openRightClick;

        private ContextMenuManager contextMenuManager;

        public void Start()
        {
            contextMenuManager = ContextMenuManager.Instance;
        }

        #region IPointerDownHandler

        public void OnPointerDown(PointerEventData eventData)
        {
            if ((openLeftClick && eventData.LeftClick()) || (openRightClick && eventData.RightClick()))
            {
                OpenContextMenu();
            }
        }

        #endregion

        public void OpenContextMenu()
        {
            contextMenuManager.DisplayMenu(GetContextMenu());
        }

        protected abstract ContextMenu GetContextMenu();
    }
}