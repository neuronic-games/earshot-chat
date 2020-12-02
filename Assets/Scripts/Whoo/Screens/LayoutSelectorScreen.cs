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
    public class LayoutSelectorScreen : Screen<LayoutSelectorScreen.Settings>
    {
        [SerializeField]
        public Button hideButton;

        [SerializeField]
        private GameObject loadingBlocker = default;

        [SerializeField]
        private RectTransform container = default;

        [SerializeField]
        private LayoutDisplay layoutDisplayTemplate = default;

        [SerializeField]
        private TextContext infoText = default;

        [Serializable]
        private class LayoutList : IDebugData
        {
            public string       Json { get; set; }
            public List<Layout> list;
        }

        public struct Settings : IScreenSettings
        {
            public Func<Layout, UniTaskVoid> OnSelected;
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

            List<Layout> list = (await Utils.GetJsonObjectAsync<LayoutList>(
                Endpoint.Base().Collection(Collections.Layout).All(), true,
                nameof(LayoutList.list))).list;
            if (list.Count == 0) infoText.SetText("No layouts are available at this moment.");
            for (var i = 0; i < list.Count; i++)
            {
                Layout        layout  = list[i];
                LayoutDisplay display = Instantiate(layoutDisplayTemplate, container);
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