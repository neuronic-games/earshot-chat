using Discord;
using DiscordAppLayer;
using UnityEngine;
using Whoo;

namespace DiscordCommon
{
    [CreateAssetMenu(menuName = "Discord/Serialized Activity")]
    public class SerializedDiscordActivity : ScriptableObject
    {
        [SerializeField] private string       activityName;
        [SerializeField] private string       details;
        [SerializeField] private string       state;
        [SerializeField] private ActivityType type;
        
        public Activity GetActivity()
        {
            DiscordApp.GetDiscordApp(out var app);
            return new Activity()
            {
                ApplicationId = WhooSettings.Instance.discordAppId,
                Name = activityName,
                Details = details,
                State = state,
                Type = type
            };
        }
    }
}