using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI.ContextMenu
{
    public class LocalClicksCatcher : MonoBehaviour, IPointerDownHandler
    {
        public UnityEvent  onCatch     = null;

        [Header("Config")]
        public bool catchLeft = false;

        public bool catchMiddle = false;
        public bool catchRight  = false;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (catchLeft && eventData.button == PointerEventData.InputButton.Left)
            {
                onCatch.Invoke();
            }
            else if (catchMiddle && eventData.button == PointerEventData.InputButton.Middle)
            {
                onCatch.Invoke();
            }
            else if (catchRight && eventData.button == PointerEventData.InputButton.Right)
            {
                onCatch.Invoke();
            }
        }
    }
}