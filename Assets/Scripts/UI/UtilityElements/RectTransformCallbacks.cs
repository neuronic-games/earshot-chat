using System;
using UnityEngine;

namespace UI.UtilityElements
{
    public class RectTransformCallbacks : MonoBehaviour
    {
        public Action<RectTransform> onRectTransformDimensionsChanged;

        private RectTransform me;
        
        private void Awake()
        {
            me = GetComponent<RectTransform>();
        }
        
        private void OnRectTransformDimensionsChange()
        {
            onRectTransformDimensionsChanged?.Invoke(me);
        }
    }
}