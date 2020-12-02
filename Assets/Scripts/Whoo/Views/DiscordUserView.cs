using System.Linq;
using Cysharp.Threading.Tasks;
using DiscordAppLayer;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Whoo.Data;

namespace Whoo.Views
{
    public class DiscordUserView : UserView, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private SpeakingWidget widget = null;

        [SerializeField]
        private StrapiAvatar strapiAvatar = null;

        [SerializeField]
        private DiscordAvatar avatar = null;

        [SerializeField, Header("Hover/Volume")]
        private bool debugLog = false;

        [SerializeField]
        private RectTransform onHoverDisplay = null;

        [SerializeField]
        private Slider volumeSlider;

        protected override void Speaking(bool speaking)
        {
            widget.SetSpeaking(speaking);
        }

        private bool _triedLoadingAvatar = false;

        public override void Refresh()
        {
            base.Refresh();
            RefreshAsync().Forget();

            //todo -- replace with custom avatar builder

            async UniTaskVoid RefreshAsync()
            {
                #region Avatar

                if (_triedLoadingAvatar) return;

                #region Volume

                UpdateSliderView();

                #endregion

                if (User.Equals(_room.RoomGroup.LocalUser))
                {
                    Profile profile = new Profile()
                    {
                        id = Build.Settings.testerInfo.profileId
                    };
                    await profile.GetAsync();
                    if (await strapiAvatar.LoadAvatar(profile.image.FirstOrDefault()?.url)) return;

                    if (User is DiscordUser user)
                    {
                        _triedLoadingAvatar = true;
                        avatar.LoadAvatar(user.DiscordUserId, user.App.ImageManager);
                    }
                }
                else
                {
                    Debug.LogWarning($"User that isn't a discord user attached to {nameof(DiscordUserView)}.");
                }

                #endregion
            }
        }

        #region IPointerHandler

        public void OnPointerEnter(PointerEventData eventData)
        {
            onHoverDisplay.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onHoverDisplay.gameObject.SetActive(false);
        }

        #endregion

        public void Awake()
        {
            onHoverDisplay.gameObject.SetActive(false);
            SetupVolumeSlider();
        }

        private const float updateInterval = 1.0f;
        private       float _lastUpdate    = 0.0f;

        public void Update()
        {
            if (Time.time > _lastUpdate + updateInterval)
            {
                UpdateSliderView();
            }
        }

        #region Volume

        private void SetupVolumeSlider()
        {
            volumeSlider.minValue = 0;
            volumeSlider.maxValue = 200;
            volumeSlider.onValueChanged.AddListener(SetLocalVolume);
        }

        private void SetLocalVolume(float newValue)
        {
            if (DiscordApp.GetDiscordApp(out DiscordApp app))
            {
                if (User is DiscordUser user)
                {
                    var volume = (byte) Mathf.Clamp(newValue, 0, 200);
                    app.VoiceManager.SetLocalVolume(user.DiscordUserId, volume);
                    if (debugLog) Debug.Log($"Volume Set to {volume}");
                }
            }

            _lastUpdate = Time.time;
        }

        private void UpdateSliderView()
        {
            if (DiscordApp.GetDiscordApp(out DiscordApp app) && User is DiscordUser dUser)
            {
                var volume = app.VoiceManager.GetLocalVolume(dUser.DiscordUserId);
                volumeSlider.value = volume;
                if (debugLog) Debug.Log($"Volume Get to {volume}");
            }
        }

        #endregion
    }
}