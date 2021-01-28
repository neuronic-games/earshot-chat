using System.Collections.Generic;
using Discord;
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

        private static Dictionary<string, Texture2D> _cache = new Dictionary<string, Texture2D>();

        public override void LoadAvatar()
        {
            if (userId == 0)
            {
                AlphaInvisible();
                return;
            }

            if (_cache.TryGetValue(userId.ToString(), out var tex))
            {
                ApplyTexture(tex);
            }
            
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
                    tex = manager.GetTexture(handleResult);
                    _cache[userId.ToString()] = tex;
                    ApplyTexture(tex);
                }
                else
                {
                    Debug.Log($"Failed to fetch avatar for user {userId}.");
                }
            });

            void AlphaVisible()
            {
                var color = image.color;
                color.a     = 1.0f;
                image.color = color;
            }
            
            void AlphaInvisible()
            {
                var color = image.color;
                color.a     = 0.0f;
                image.color = color;
            }

            void ApplyTexture(Texture2D value)
            {
                if (image == null) return;
                image.texture = value;
                AlphaVisible();
                SetUvRectToVerticalFlipped();
                onAvatarAvailable.Invoke(value);
            }
        }

        private void SetUvRectToVerticalFlipped()
        {
            var uvRect = image.uvRect;
            uvRect.y      = 1;
            uvRect.height = -1;
            image.uvRect  = uvRect;
        }
    }
}