using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class AvatarViewInLobby : MonoBehaviour
    {
        [SerializeField] private Image iconImage;

        private void Start()
        {
            iconImage.sprite = UserDTO.AvatarSprite;
        }
    }
}