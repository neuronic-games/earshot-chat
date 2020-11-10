using UnityEngine;

namespace UI
{
    public interface IHoverHandler
    {
        void HoverEnter();
        void HoverExit();
    }

    public class HoverDistributor : MonoBehaviour
    {
        public enum PriorityType
        {
            All,
            Highest,
            Lowest
        }
    }
}