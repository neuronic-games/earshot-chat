using System;
using System.Collections.Generic;

namespace DiscordAppLayer
{
    public class LocalDiscordUser : DiscordUser
    {
        public void SetDiscordGroup(DiscordNetworkGroup group)
        {
            DiscordGroup = group;
        }

        public LocalDiscordUser() : base(null)
        {
        }

        public LocalDiscordUser(long userId, int permLevel, string name, DiscordNetworkGroup @group) : base(userId,
            permLevel, name, @group)
        {
        }

        public void SetCustomProperties(IReadOnlyDictionary<string, string> properties, Action onImplemented,
            DiscordNetworkGroup                                             group)
        {
            SetDiscordGroup(group);
            base.SetCustomProperties(properties, onImplemented);
        }

        public void DeleteCustomProperties(IReadOnlyList<string> properties, Action onImplemented,
            DiscordNetworkGroup                                  group)
        {
            SetDiscordGroup(group);
            base.DeleteCustomProperties(properties, onImplemented);
        }
    }
}