using UnityEngine.EventSystems;

namespace UI.Manipulators
{
    public class UIDraggable : UIManipulator, IDragHandler
    {
        public void OnDrag(PointerEventData eventData)
        {
            target.anchoredPosition += eventData.delta;
        }
    }
}