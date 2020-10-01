using System;
using UnityEngine;
using Discord;
using UnityEngine.Events;

namespace DiscordCommon
{
    public class DiscordController : MonoBehaviour
    {
        #region Serializable

        public int instanceId = 0;

        #endregion

        #region Events

        [Serializable]
        public class DiscordCreatedEvent : UnityEvent<Discord.Discord>
        {
        }

        public DiscordCreatedEvent onCreated;

        #endregion

        #region Unity Callbacks

        void Start()
        {
            Discord.Discord discord =
                DiscordApp.InitializeApp(760222636701253652, (ulong) CreateFlags.Default, instanceId);

            if (DiscordApp.Initialized)
            {
                discord.SetLogHook(LogLevel.Info, LogDiscord);

                onCreated?.Invoke(discord);
            }
        }

        public void OnDestroy()
        {
            if (DiscordApp.Initialized)
            {
                DiscordApp.DestroyApp();
            }
        }

        void Update()
        {
            if (DiscordApp.Initialized)
            {
                DiscordApp.Update();
            }
        }

        #endregion

        #region Callbacks

        private void LogDiscord(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Error:
                    Debug.LogError($"Discord:{level} - {message}");
                    break;
                case LogLevel.Warn:
                    Debug.LogWarning($"Discord:{level} - {message}");
                    break;
                case LogLevel.Info:
                    Debug.Log($"Discord:{level} - {message}");
                    break;
                case LogLevel.Debug:
                    Debug.Log($"Discord:{level} - {message}");
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(level), level, null);
            }
        }

        #endregion
    }
}