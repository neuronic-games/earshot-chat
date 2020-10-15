using System;
using AppLayer;
using Discord;
using UnityEngine;
using UnityEngine.Events;
using Whoo;

namespace DiscordAppLayer
{
    [DefaultExecutionOrder(-100)]
    public class DiscordController : MonoBehaviour
    {
        #region Serializable

        public bool debugLog     = true;
        public bool logAllEvents = true;
        public int  instanceId   = 0;

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
            Debug.Log($"Started.");
            App = DiscordApp.InitializeApp(WhooSettings.Instance.discordAppId,
                (ulong) WhooSettings.Instance.discordCreateFlags, instanceId);

            if (debugLog)
            {
                AppLayer.AppLayer.Set(new LogAppLayer(App, logAllEvents));
            }
            else
            {
                AppLayer.AppLayer.Set(App);
            }

            App.Discord.SetLogHook(LogLevel.Info, LogDiscord);

            onCreated?.Invoke(App);
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