using Discord;
using DiscordAppLayer;
using UnityEngine;
using Whoo;

namespace DiscordCommon
{
    [CreateAssetMenu(menuName = "Discord/Serialized Activity")]
    public class SerializedDiscordActivity : ScriptableObject
    {
        [SerializeField]
        private string activityName = string.Empty;

        [SerializeField]
        private string details = string.Empty;

        [SerializeField]
        private string state = string.Empty;

        [SerializeField]
        private ActivityType type = ActivityType.Watching;

        public Activity GetActivity()
        {
            DiscordApp.GetDiscordApp(out var app);
            return new Activity()
            {
                ApplicationId = WhooSettings.Instance.discordAppId,
                Name          = activityName,
                Details       = details,
                State         = state,
                Type          = type
            };
        }
    }
}