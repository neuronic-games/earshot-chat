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
    public class StrapiUserView : UserView, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private SpeakingWidget widget = null;

        [SerializeField]
        private StrapiAvatar strapiAvatar = null;

        [SerializeField]
        private DiscordAvatar discordAvatar = null;

        [SerializeField]
        private GameObject colorPanel = null;

        [SerializeField, Header("Hover/Volume")]
        private RectTransform onHoverDisplay = null;

        [SerializeField]
        private Slider volumeSlider;

        protected override void Speaking(bool speaking)
        {
            widget.SetSpeaking(speaking);
        }

        private bool _triedLoadingAvatar = false;

        public override async UniTask Refresh()
        {
            //todo -- replace with custom avatar builder
            await base.Refresh();

            //update volume slider
            UpdateSliderView();

            #region Avatar

            if (_triedLoadingAvatar) return;

            //load avatar from strapi 
            if (strapiAvatar.gameObject.activeSelf)
            {
                string avatarUrl;
                StrapiUser strapiUser =
                    Build.AuthContexts.OfType<StrapiAuthenticatedUser>().FirstOrDefault()?.StrapiUser;
                Profile profile = new Profile();

                //fetch profile based on identity
                if (User.Equals(Room.RoomGroup.LocalUser) && strapiUser != null)
                {
                    //self -- load from user's profile
                    profile.id = Build.AuthContexts.OfType<StrapiAuthenticatedUser>().FirstOrDefault()?.StrapiUser.id;
                    await profile.GetAsync();
                }
                else
                {
                    //someone else -- attempt to get profile from platform id
                    string profileUri = Endpoint.Base().
                                                 Collection(Collections.Profile).
                                                 Equals((Profile p) => p.platform_unique_id, User.UniqueId).
                                                 Limit(1);
                    profile = (await Utils.GetJsonArrayAsync<Profile>(profileUri)).List.FirstOrDefault();
                }

                if (profile == null) avatarUrl = string.Empty;
                else
                {
                    avatarUrl = profile.image[0].url;
                }

                await strapiAvatar.LoadAvatar(avatarUrl);
            }

            //load avatar from discord
            if (discordAvatar.gameObject.activeSelf && User is DiscordUser discordUser)
            {
                discordAvatar.LoadAvatar(discordUser.DiscordUserId, discordUser.App.ImageManager);
            }

            _triedLoadingAvatar = true;

            #endregion
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

        // Avatars can have different background colors to indicate groups.
        // This is defined in the data model.
        private void SetColor(Color color)
        {
            colorPanel.GetComponent<Image>().color = color;
        }

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
            }
        }

        #endregion
    }
}