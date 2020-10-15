﻿using Discord;
using UnityEngine;

namespace UI
{
    public class DiscordAvatar : AvatarBase
    {
        public uint avatarSize = 64;
        
        public long userId = 0;
        public ImageManager manager;

        public void LoadAvatar(long userId, ImageManager manager)
        {
            this.userId = userId;
            this.manager = manager;
            LoadAvatar();
        }

        public override void LoadAvatar()
        {
            if (userId == 0) return;
            
            var handle = new ImageHandle()
            {
                Type = ImageType.User,
                Id = userId,
                Size = avatarSize
            };
            
            manager.Fetch(handle, false, (result, handleResult) =>
            {
                if (result == Result.Ok)
                {
                    var tex = manager.GetTexture(handleResult);
                    image.texture = tex;
                    AlphaVisible();
                    onAvatarAvailable.Invoke(tex);
                }
                else
                {
                    Debug.Log($"Failed to fetch avatar for user {userId}.");
                }
            });
            

            void AlphaVisible()
            {
                var color = image.color;
                color.a = 1.0f;
                image.color = color;
            }
        }
    }
}