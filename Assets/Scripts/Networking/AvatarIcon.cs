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
        [SerializeField] private Image background;

        public Image IconImage => iconImage;
        public Image Background => background;
        
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
            Debug.Log("image name - " + image.name);

            var color = background.color;
            UserDTO.BackgroundColor = color;
        }
    }
}
