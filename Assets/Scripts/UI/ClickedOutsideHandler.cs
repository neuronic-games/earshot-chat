using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class ClickedOutsideHandler : MonoBehaviour
    {
        public UnityEvent onClickedOutside;
        
        private void OnEnable()
        {
            OnDisable();
            GlobalClicksCatcher.OnPointerDown += ClickedOutside;
        }

        private void OnDisable()
        {
            GlobalClicksCatcher.OnPointerDown -= ClickedOutside;
        }

        private void ClickedOutside(object sender, PointerEventData e)
        {
            onClickedOutside.Invoke();
        }
    }
}