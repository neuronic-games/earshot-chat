using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UI;
using UI.Screens;
using UnityEngine;
using UnityEngine.UI;
using Whoo.Data;

namespace Whoo.Screens
{
    public class RoomSelectorScreen : Screen<RoomSelectorScreen.Settings>
    {
        [SerializeField]
        public Button hideButton = default;

        [SerializeField]
        private GameObject loadingBlocker = default;

        [SerializeField]
        private RectTransform container = default;

        [SerializeField]
        private RoomModelDisplay layoutDisplayTemplate = default;

        [SerializeField]
        private TextContext infoText = default;

        [Serializable]
        private class RoomModelList : IDebugData
        {
            public string          Json { get; set; }
            public List<RoomModel> list;
        }

        public struct Settings : IScreenSettings
        {
            public Func<RoomModel, UniTaskVoid> OnSelected;
            public string                       ProfileId;
        }

        public override void Setup(ref Settings settings)
        {
            infoText.SetText(string.Empty);
            base.Setup(ref settings);
            SetupAsync().Forget();
        }

        private async UniTaskVoid SetupAsync()
        {
            loadingBlocker.SetActive(true);

            container.ClearChildren(false);

            if (string.IsNullOrEmpty(currentSettings.ProfileId))
            {
                Debug.Log($"{nameof(SetupAsync)}: profile id is empty.");
                infoText.SetText("Guests can't save rooms.");
                return;
            }

            var roomListEndpoint = Endpoint.Base().
                                                   Collection(Collections.Room).
                                                   Equals((RoomModel r) => r.owner.id, currentSettings.ProfileId);
            List<RoomModel> list = (await Utils.GetJsonObjectAsync<RoomModelList>(
                roomListEndpoint, true,
                nameof(RoomModelList.list))).list;
            if (list.Count == 0) infoText.SetText("You have no rooms saved.");
            for (var i = 0; i < list.Count; i++)
            {
                RoomModel        layout  = list[i];
                RoomModelDisplay display = Instantiate(layoutDisplayTemplate, container);
                display.LoadLayout(layout, () =>
                {
                    currentSettings.OnSelected?.Invoke(layout).Forget();
                    Hide();
                });
            }

            loadingBlocker.SetActive(false);
        }

        public void Awake()
        {
            hideButton.onClick.AddListener(Hide);
        }

        public override void Refresh()
        {
        }
    }
}