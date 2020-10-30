using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Tooltip
{
    public abstract class TooltipContent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        protected TooltipManager tpManager;

        public virtual void Start()
        {
            if (tpManager == null)
            {
                tpManager = TooltipManager.Instance;
            }
        }

        public abstract string GetTooltipText();

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (tpManager != null) tpManager.SetContent(GetTooltipText());
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (tpManager != null) tpManager.Hide();
        }
    }
}