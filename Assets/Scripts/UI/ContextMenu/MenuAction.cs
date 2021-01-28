using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.ContextMenu
{
    public class MenuAction : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
    {
        public TextMeshProUGUI actionName = null;

        private ContextMenuManager contextMenuManager;
        private ContextMenu.Entry  currentEntry;
        private Animator           animator;

        public void Awake()
        {
            animator = GetComponent<Animator>();
        }

        public bool Setup(ContextMenuManager manager, ContextMenu.Entry actionEntry)
        {
            if (actionEntry.EntryType != ContextMenu.EntryType.Action) return false;
            contextMenuManager      = manager;
            currentEntry = actionEntry;

            actionName.text = currentEntry.Name;
            return true;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                contextMenuManager.NotifyClicked(this);
                currentEntry.Action?.Invoke();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(animator != null) animator.Play("In");
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if(animator != null) animator.Play("Out");
        }
    }
}