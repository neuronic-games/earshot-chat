using System.Linq;
using MLAPI;
using MLAPI.NetworkVariable;
using TMPro;
using UnityEngine;

namespace Networking
{
    public class AvatarPlayer : NetworkBehaviour
    {
        
        [SerializeField] private AvatarCollection avatarCollection;
        [SerializeField] private SpriteRenderer iconSprite;
        
        private NetworkVariableString userName =
            new NetworkVariableString(
                new NetworkVariableSettings() {WritePermission = NetworkVariablePermission.OwnerOnly}, "username");
 
        private NetworkVariableString avatarIconName =
            new NetworkVariableString(
                new NetworkVariableSettings() {WritePermission = NetworkVariablePermission.OwnerOnly}, "avatarname");

        private TextMeshPro tmp;

        public Sprite AvatarSprite => iconSprite.sprite;
        
        private void Awake()
        {
            tmp = gameObject.GetComponentInChildren<TextMeshPro>();
        }

        private void Start()
        {
            if (!IsLocalPlayer)
                return;
            var currentUserName = UserDTO.Username;
            userName.Value = currentUserName;
            avatarIconName.Value = UserDTO.AvatarSprite.name;
        }

        

        private void Update()
        {
            if (IsLocalPlayer)
            {
                var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0);
                transform.position += move * 5f * Time.deltaTime;
            }

            tmp.text = userName.Value;
            SetAvatarSprite();
        }
        
        private void SetAvatarSprite()
        {
            Sprite sprite = avatarCollection.AvatarVariants.FirstOrDefault(a => a.name == avatarIconName.Value);
            iconSprite.sprite = sprite;
        }
    }
}