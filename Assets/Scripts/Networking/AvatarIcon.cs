using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class AvatarIcon : MonoBehaviour
    {
        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(ChooseAvatar);
        }

        private void ChooseAvatar()
        {
            Debug.Log("clicked over avatar");
            var image = GetComponent<Image>().sprite.name;
            Debug.Log(image);
        }
    }
}
