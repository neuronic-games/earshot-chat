using Discord;
using DiscordAppLayer;
using UnityEngine;
using Whoo;

namespace DiscordCommon
{
    [CreateAssetMenu(menuName = "Discord/Serialized Activity")]
    public class SerializedDiscordActivity : ScriptableObject
    {
        /// <summary>
        /// Not displayed to the user.
        /// </summary>
        [SerializeField, Header("Basic Description")]
        private string activityName = string.Empty;

        /// <summary>
        /// Lighter-shade text that's meant to further explain the "State".
        /// </summary>
        [SerializeField]
        private string details = string.Empty;

        /// <summary>
        /// Phrases like "Playing Solo", "Queueing", "Idleing", "Matchmaking", "Playing Random".
        /// </summary>
        [SerializeField]
        private string state = string.Empty;

        /// <summary>
        /// Not required to be set explicitly. Only useful for callbacks.
        /// </summary>
        [SerializeField]
        private ActivityType type = ActivityType.Watching;

        /*     Explanation:
         *     In discord developer portal, you can upload asset images and give them a "key".
         *     That's the key field below. And the tooltip field is for text displayed when user hovers on image.
         *     The activity-view has two variations for images: large and small.
         */
        [Header("Assets"), SerializeField]
        private string LargeTooltipText = string.Empty;

        [SerializeField]
        private string LargeImageKey = string.Empty;

        [SerializeField]
        private string SmallTooltipText = string.Empty;

        [SerializeField]
        private string SmallImageKey = string.Empty;

        public Activity GetActivity()
        {
            return new Activity()
            {
                ApplicationId = WhooSettings.Instance.discordAppId,
                Name          = activityName,
                Details       = details,
                State         = state,
                Type          = type,
                Assets = new ActivityAssets()
                {
                    LargeImage = LargeImageKey,
                    SmallImage = SmallImageKey,
                    LargeText = LargeTooltipText,
                    SmallText = SmallTooltipText
                }
            };
        }
    }
}