using Discord;
using UnityEngine;

namespace Whoo
{
    [CreateAssetMenu(menuName = "Whoo/Settings", order = 0)]
    public class WhooSettings : ScriptableSingleton<WhooSettings>
    {
        public long        discordAppId;
        public CreateFlags discordCreateFlags;
    }
}