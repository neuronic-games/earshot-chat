using UI.UtilityElements;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.ContextMenu
{
    public abstract class ContextMenuArea : MonoBehaviour, IClickDownHandler
    {
        [Header("Config")]
        public bool openLeftClick;

        public bool openRightClick;

        private ContextMenuManager contextMenuManager;

        public virtual void Start()
        {
            contextMenuManager = ContextMenuManager.Instance;
        }

        #region IClickHandler


        public void LeftClick(PointerEventData ptrData)
        {
            if(openLeftClick) OpenContextMenu();
        }

        public void RightClick(PointerEventData ptrData)
        {
            if(openRightClick) OpenContextMenu();
        }

        public void MiddleClick(PointerEventData ptrData)
        {
            
        }

        #endregion

        public void OpenContextMenu()
        {
            contextMenuManager.DisplayMenu(GetContextMenu());
        }

        protected abstract ContextMenu GetContextMenu();
    }
}