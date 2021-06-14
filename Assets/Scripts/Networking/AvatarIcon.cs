using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class AvatarIcon : MonoBehaviour
    {
        [SerializeField] private Image iconImage;

        public Image IconImage => iconImage;
        
        private void Awake()
        {
            var button = GetComponentInChildren<Button>();
            button.onClick.AddListener(ChooseAvatar);
        }

        private void ChooseAvatar()
        {
            Debug.Log("clicked over avatar");
            var image = iconImage.sprite;
            UserDTO.AvatarSprite = image;
            Debug.Log(image.name);
            
            
        }
    }
}
