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

        public struct Settings : IScreenSettings
        {
            public Func<RoomModel, UniTaskVoid> OnSelected;
            public string                       ProfileId;
        }

        public override async UniTask Setup(Settings settings)
        {
            infoText.SetText(string.Empty);
            await base.Setup(settings);

            loadingBlocker.SetActive(true);

            container.ClearChildren(false);

            if (string.IsNullOrEmpty(CurrentSettings.ProfileId))
            {
                Debug.Log($"{nameof(Setup)}: profile id is empty.");
                infoText.SetText("Guests can't save rooms.");
                return;
            }

            var roomListEndpoint = Endpoint.Base().
                                            Collection(Collections.Room).
                                            Equals((RoomModel r) => r.owner.id, CurrentSettings.ProfileId);
            List<RoomModel> list = (await Utils.GetJsonArrayAsync<RoomModel>(
                roomListEndpoint, string.Empty)).List;

            if (list == null || list.Count == 0) infoText.SetText("You have no rooms saved.");

            if (list != null)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    RoomModel        layout  = list[i];
                    RoomModelDisplay display = Instantiate(layoutDisplayTemplate, container);
                    display.LoadLayout(layout, () =>
                    {
                        CurrentSettings.OnSelected?.Invoke(layout).Forget();
                        Hide().Forget();
                    });
                }
            }

            loadingBlocker.SetActive(false);
        }

        public void Awake()
        {
            hideButton.onClick.AddListener(() => Hide().Forget());
        }
    }
}