using System;
using UnityEngine;
using Random = UnityEngine.Random;

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
                var iconObject = InstantiateAvatarIcon();

                SetAvatarIcon(iconObject, icon);

                var randomBgColor = ChooseRandomBgColor();

                SetBackgroundColor(iconObject, randomBgColor);
            }
        }

        private GameObject InstantiateAvatarIcon()
        {
            var iconObject = Instantiate(avatarIconPrefab, Vector3.zero, Quaternion.identity);
            iconObject.transform.SetParent(avatarsParent);
            return iconObject;
        }

        private static void SetBackgroundColor(GameObject iconObject, Color randomBgColor)
        {
            var backGround = iconObject.GetComponent<AvatarIcon>().Background;
            backGround.color = randomBgColor;
        }

        private Color ChooseRandomBgColor()
        {
            var r = Random.Range(0, 1f);
            var b = Random.Range(0, 1f);
            var g = Random.Range(0, 1f);
            //this random color should be chosen by color picker in the future
            var randomBgColor = new Color(r, g, b);
            return randomBgColor;
        }

        private void SetAvatarIcon(GameObject iconObject, Sprite icon)
        {
            var image = iconObject.GetComponent<AvatarIcon>().IconImage;
            image.sprite = icon;
        }
    }
}
