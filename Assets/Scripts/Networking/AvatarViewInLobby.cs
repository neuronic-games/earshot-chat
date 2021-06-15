using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class AvatarViewInLobby : MonoBehaviour
    {
        [SerializeField] private Image iconImage;
        [SerializeField] private Image bgColor;

        public Image IconSprite => iconImage;
        public Image BgColor => bgColor;
    }
}