using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Whoo;

namespace UI
{
    public interface IClickHandler
    {
        void LeftClick();
        void RightClick();
        void MiddleClick();
    }

    public class ClickDistributor : MonoBehaviour, IPointerDownHandler
    {
        private List<IClickHandler> handlers = new List<IClickHandler>();

        public void Awake()
        {
            transform.parent.GetComponentsInChildren(handlers);
        }

        public void Register(IClickHandler handler)
        {
            if (!handlers.Contains(handler)) handlers.Add(handler);
        }

        public void Unregister(IClickHandler handler)
        {
            handlers.Remove(handler);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.LeftClick())
            {
                for (var i = 0; i < handlers.Count; i++)
                {
                    handlers[i].LeftClick();
                }
            }
            else if (eventData.RightClick())
            {
                for (var i = 0; i < handlers.Count; i++)
                {
                    handlers[i].RightClick();
                }
            }
            else if (eventData.MiddleClick())
            {
                for (var i = 0; i < handlers.Count; i++)
                {
                    handlers[i].MiddleClick();
                }
            }
        }
    }
}