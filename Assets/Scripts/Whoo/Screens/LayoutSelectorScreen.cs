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

        public struct Settings : IScreenSettings
        {
            public Func<Layout, UniTaskVoid> OnSelected;
        }

        public override async UniTask Setup(Settings settings)
        {
            infoText.SetText(string.Empty);
            await base.Setup(settings);

            loadingBlocker.SetActive(true);

            container.ClearChildren(false);

            string       uri  = Endpoint.Base().Collection(Collections.Layout).All();
            List<Layout> list = (await Utils.GetJsonArrayAsync<Layout>(uri)).List;

            if (list == null || list.Count == 0) infoText.SetText("No layouts are available at this moment.");

            if (list != null)
            {
                for (var i = 0; i < list.Count; i++)
                {
                    Layout        layout  = list[i];
                    LayoutDisplay display = Instantiate(layoutDisplayTemplate, container);
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
