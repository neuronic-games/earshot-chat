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
    public class RoomModelDisplay : MonoBehaviour
    {
        public RawImage    image        = default;
        public TextContext personalName = default;
        public TextContext layoutName   = default;
        public TextContext capacity     = default;
        public Button      selectButton;
        public Button      copyableId;

        private Action    _whenSelected;
        private RoomModel _roomModel;

        public void Awake()
        {
            selectButton.onClick.AddListener(Selected);
            copyableId.onClick.AddListener(() =>
            {
                if (_roomModel != null && !string.IsNullOrEmpty(_roomModel.id))
                {
                    Utils.SetClipboardText(_roomModel.id);
                }
            });
        }

        private void Selected()
        {
            _whenSelected?.Invoke();
        }

        public void LoadLayout(RoomModel room, Action selected)
        {
            gameObject.SetActive(true);
            _whenSelected = selected;
            _roomModel    = room;
            if (room.layout != null)
            {
                LoadImage(room.layout.image.FirstOrDefault()?.url).Forget();
                capacity.SetText(room.layout.capacity.ToString());
                layoutName.SetText(room.layout.name);
            }

            personalName.SetText(room.name);
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