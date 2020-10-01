using System;
using Discord;
using UnityEngine.Assertions;

namespace DiscordCommon
{
    public static class DiscordApp
    {
        #region Events

        public static event Action OnInitialized;

        #endregion

        #region Properties

        private static bool _initialized;

        public static bool Initialized
        {
            get
            {
                Assert.IsNotNull(_discord);
                return _initialized;
            }
            set
            {
                Assert.IsNotNull(_discord);
                _initialized = value;
                if (_initialized) OnInitialized?.Invoke();
            }
        }

        private static Discord.Discord _discord;
        public static  Discord.Discord Discord => _discord;

        #region Manager Caches

        private static VoiceManager _voiceManager = null;

        public static VoiceManager VoiceManager
        {
            get
            {
                Assert.IsTrue(Initialized);
                if (_voiceManager == null)
                {
                    _voiceManager = _discord.GetVoiceManager();
                }

                return _voiceManager;
            }
        }

        private static LobbyManager _lobbyManager = null;

        public static LobbyManager LobbyManager
        {
            get
            {
                Assert.IsTrue(Initialized);
                if (_lobbyManager == null)
                {
                    _lobbyManager = _discord.GetLobbyManager();
                }

                return _lobbyManager;
            }
        }

        private static OverlayManager _overlayManager = null;

        public static OverlayManager OverlayManager
        {
            get
            {
                Assert.IsTrue(Initialized);
                if (_overlayManager == null)
                {
                    _overlayManager = _discord.GetOverlayManager();
                }

                return _overlayManager;
            }
        }


        
        #endregion

        #endregion

        #region Methods

        #region Lifetime

        public static Discord.Discord InitializeApp(long clientId, ulong flags, int instanceId = 0)
        {
            Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", instanceId.ToString());
            _discord = new Discord.Discord(clientId, flags);

            Initialized = true;

            return _discord;
        }

        public static void Update()
        {
            _discord.RunCallbacks();
        }

        public static void DestroyApp()
        {
            _discord.Dispose();
            Initialized = false;
        }

        #endregion

        #endregion
    }
}