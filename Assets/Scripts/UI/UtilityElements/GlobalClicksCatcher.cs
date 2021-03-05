using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.UtilityElements
{
    public class GlobalClicksCatcher : MonoBehaviour, IPointerDownHandler
    {
        public static event EventHandler<PointerEventData> OnPointerDown;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            OnPointerDown?.Invoke(this, eventData);
        }
    }
}