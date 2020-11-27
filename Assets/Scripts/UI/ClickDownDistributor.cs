using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Whoo;

namespace UI
{
    public interface IClickDownHandler
    {
        void LeftClick(PointerEventData   ptrData);
        void RightClick(PointerEventData  ptrData);
        void MiddleClick(PointerEventData ptrData);
    }
    public class ClickDownDistributor : MonoBehaviour, IPointerDownHandler
    {
        private List<IClickDownHandler> handlers = new List<IClickDownHandler>();

        public void Awake()
        {
            transform.parent.GetComponentsInChildren(handlers);
        }

        public void Register(IClickDownHandler handler)
        {
            if (!handlers.Contains(handler)) handlers.Add(handler);
        }

        public void Unregister(IClickDownHandler handler)
        {
            handlers.Remove(handler);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.LeftClick())
            {
                for (var i = 0; i < handlers.Count; i++)
                {
                    handlers[i].LeftClick(eventData);
                }
            }
            else if (eventData.RightClick())
            {
                for (var i = 0; i < handlers.Count; i++)
                {
                    handlers[i].RightClick(eventData);
                }
            }
            else if (eventData.MiddleClick())
            {
                for (var i = 0; i < handlers.Count; i++)
                {
                    handlers[i].MiddleClick(eventData);
                }
            }
        }
    }
}