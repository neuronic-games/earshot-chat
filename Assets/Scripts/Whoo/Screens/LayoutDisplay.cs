using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Whoo.Data;

namespace Whoo.Screens
{
    public class LayoutDisplay : MonoBehaviour
    {
        public RawImage    image      = default;
        public TextContext layoutName = default;
        public TextContext capacity   = default;
        public Button      selectButton;

        private Action _whenSelected;
        private Layout _layout;

        public void Awake()
        {
            selectButton.onClick.AddListener(Selected);
        }

        private void Selected()
        {
            _whenSelected?.Invoke();
        }

        public void LoadLayout(Layout layout, Action selected)
        {
            gameObject.SetActive(true);
            _whenSelected = selected;
            _layout       = layout;
            LoadImage(layout.image.FirstOrDefault()?.url).Forget();
            capacity.SetText(layout.capacity.ToString());
            layoutName.SetText(layout.name);
        }

        public async UniTaskVoid LoadImage(string url)
        {
            selectButton.interactable = false;
            Texture layoutImage =
                await Utils.LoadPossibleWhooImage(url, false, true, gameObject.GetCancellationTokenOnDestroy());

            if (layoutImage != null)
            {
                image.ApplyTextureAndFit(layoutImage);
            }

            selectButton.interactable = true;
        }
    }
}