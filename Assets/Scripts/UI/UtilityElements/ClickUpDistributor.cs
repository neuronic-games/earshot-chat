using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.UtilityElements
{

    public interface IClickUpHandler
    {
        void Up();
    }

    public class ClickUpDistributor : MonoBehaviour, IPointerUpHandler
    {
        private List<IClickUpHandler> handlers = new List<IClickUpHandler>();

        public void Awake()
        {
            transform.parent.GetComponentsInChildren(handlers);
        }

        public void Register(IClickUpHandler handler)
        {
            if (!handlers.Contains(handler)) handlers.Add(handler);
        }

        public void Unregister(IClickUpHandler handler)
        {
            handlers.Remove(handler);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            for (var i = 0; i < handlers.Count; i++)
            {
                handlers[i].Up();
            }
        }
    }
}