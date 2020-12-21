using Cysharp.Threading.Tasks;
using Discord;
using DiscordAppLayer;
using UnityEngine;
using UnityEngine.UI;
using Screen = UI.Screens.Screen;

namespace Whoo.Screens
{
    public class SettingsScreen : Screen
    {
        #region Serialized

        [SerializeField]
        private Button closeButton = default;

        [SerializeField]
        private Button openDiscordSettingsButton = default;

        [SerializeField]
        private Animator animator = default;

        [SerializeField]
        private Slider selfVolume = default;

        #endregion

        #region Screen

        public override async UniTask Hide()
        {
            animator.Play("Out");
        }

        public override async UniTask Display()
        {
            animator.Play("In");
            Refresh();
        }

        public override async UniTask Refresh()
        {
            if (DiscordApp.GetDiscordApp(out DiscordApp app))
            {
                float volume = app.VoiceManager.GetLocalVolume(app.LocalUser.Id);
                selfVolume.value = volume;
            }
        }

        #endregion

        private void Awake()
        {
            closeButton.onClick.AddListener(() => Hide().Forget());
            selfVolume.minValue = 0;
            selfVolume.maxValue = 200;
            selfVolume.onValueChanged.AddListener(SetLocalVolume);
            openDiscordSettingsButton.onClick.AddListener(OpenDiscordSettings);
        }

        private void OpenDiscordSettings()
        {
            if (DiscordApp.GetDiscordApp(out DiscordApp app))
            {
                app.OverlayManager.OpenVoiceSettings(OverlayCallback);
            }

            void OverlayCallback(Result result)
            {
            }
        }

        private void SetLocalVolume(float newValue)
        {
            if (DiscordApp.GetDiscordApp(out DiscordApp app))
            {
                app.VoiceManager.SetLocalVolume(app.LocalUser.Id, (byte) Mathf.Clamp(newValue, 0, 200));
            }
        }
    }
}