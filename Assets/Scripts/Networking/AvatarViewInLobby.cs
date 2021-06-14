using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class AvatarViewInLobby : MonoBehaviour
    {
        [SerializeField] private Image iconImage;

        public Image IconSprite => iconImage;

    }
}