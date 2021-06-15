using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class AvatarViewInLobby : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI userName;
        [SerializeField] private Image iconImage;
        [SerializeField] private Image bgColor;

        public TextMeshProUGUI Name => userName;
        public Image Icon => iconImage;
        public Image BgColor => bgColor;
    }
}