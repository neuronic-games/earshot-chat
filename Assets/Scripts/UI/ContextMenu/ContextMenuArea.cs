using UnityEngine;

namespace UI.ContextMenu
{
    public abstract class ContextMenuArea : MonoBehaviour, IClickHandler
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


        public void LeftClick()
        {
            if(openLeftClick) OpenContextMenu();
        }

        public void RightClick()
        {
            if(openRightClick) OpenContextMenu();
        }

        public void MiddleClick()
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