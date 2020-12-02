using UnityEngine.EventSystems;
using UnityEngine.UIElements;

namespace UI.Manipulators
{
    public class UIDraggable : UIManipulator, IDragHandler
    {
        public virtual void OnDrag(PointerEventData eventData)
        {
            target.anchoredPosition += eventData.delta;
        }
    }
}