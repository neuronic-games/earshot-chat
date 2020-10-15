﻿using System;
using System.Collections.Generic;

namespace DiscordAppLayer
{
    public class LocalDiscordUser : DiscordUser
    {
        public void SetDiscordGroup(DiscordNetworkGroup group)
        {
            DiscordGroup = group;
        }

        public LocalDiscordUser(DiscordApp app) : base(app, null)
        {
        }

        public LocalDiscordUser(long userId, int permLevel, string name, DiscordNetworkGroup @group, DiscordApp app) : base(userId,
            permLevel, name, app, @group)
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