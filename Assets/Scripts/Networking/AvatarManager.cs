using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Networking
{
    public class AvatarManager : MonoBehaviour
    {
        [SerializeField] private Transform avatarsParent;
        [SerializeField] private GameObject avatarIconPrefab;
        [SerializeField] private AvatarCollection avatarCollection;
        
        private void Start()
        {
            PopulateAvatarIcons();
        }

        private void PopulateAvatarIcons()
        {
            foreach (var icon in avatarCollection.AvatarVariants)
            {
                var iconObject = Instantiate(avatarIconPrefab, Vector3.zero, Quaternion.identity);
                iconObject.transform.SetParent(avatarsParent);
                var image =  iconObject.GetComponent<Image>();
                image.sprite = icon;
            }
        }
        
        
    }
}
