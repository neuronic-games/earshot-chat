using DiscordAppLayer;
using UI;
using UnityEngine;

namespace Whoo.Views
{
    public class DiscordUserView : UserView
    {
        [SerializeField]
        private SpeakingWidget widget = null;

        [SerializeField]
        private DiscordAvatar avatar = null;
        
        protected override void Speaking(bool speaking)
        {
            widget.SetSpeaking(speaking);
        }

        public override void Refresh()
        {
            base.Refresh();
            if (User is DiscordUser user)
            {
                avatar.LoadAvatar(user.DiscordUserId, user.App.ImageManager);
            }
            else
            {
                Debug.LogWarning($"User that isn't a discord user attached to {nameof(DiscordUserView)}.");
            }
        }

        protected override void CustomPropertiesUpdated()
        {
            base.CustomPropertiesUpdated();
            gameObject.SetActive(User.CustomProperties.ContainsKey(Constants.Sitting));
        }
    }
}