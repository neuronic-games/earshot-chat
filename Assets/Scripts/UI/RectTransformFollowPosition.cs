using UnityEngine;

namespace UI
{
    [ExecuteInEditMode]
    public class RectTransformFollowPosition : MonoBehaviour
    {
        public RectTransform locked = null;
        public RectTransform target = null;

        public Vector3 offset = Vector3.zero;

        void Update()
        {
            var targetPosition = target.position + offset;
            if (locked.position == targetPosition) return;
            locked.position = targetPosition;
        }
    }
}