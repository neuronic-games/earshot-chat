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
        public class DiscordCreatedEvent : UnityEvent<DiscordApp>
        {
        }

        public DiscordCreatedEvent onCreated;

        #endregion

        #region Unity Callbacks

        private DiscordApp App;

        void Start()
        {
            App = DiscordApp.InitializeApp(760222636701253652, (ulong) CreateFlags.Default, instanceId);

            AppLayer.AppLayer.Set(App); //todo -- logging
            
            if (App.Initialized)
            {
                App.Discord.SetLogHook(LogLevel.Info, LogDiscord);

                onCreated?.Invoke(App);
            }
        }

        public void OnDestroy()
        {
            if (App.Initialized)
            {
                App.DestroyApp();
            }
        }

        void Update()
        {
            if (App.Initialized)
            {
                App.Update();
            }
        }

        #endregion

        #region Log Hook

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