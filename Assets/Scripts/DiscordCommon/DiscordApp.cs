using System;
using System.Collections.Generic;
using AppLayer;
using Discord;
using DiscordCommon.Interfaces;
using UnityEngine.Assertions;

namespace DiscordCommon
{
    /// <summary>
    /// Discord-based implementation of the IAppLayer service. 
    /// </summary>
    public class DiscordApp : IAppLayer
    {
        #region Constructor

        public DiscordApp(long clientId, ulong flags, int instanceId)
        {
            Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", instanceId.ToString());
            _discord = new Discord.Discord(clientId, flags);
            
            Distributor = new EventDistributor(this);
            
            Distributor.SubscribeGameSDK();

            Initialized = true;
        }

        #endregion

        //Contains fields, properties, events, etc.

        #region Data Members (Has Properties)

        #region Own Events

        public event Action OnInitialized;

        #endregion

        #region Readonly variables

        private readonly EventDistributor Distributor;

        #endregion

        #region Properties

        private bool _initialized;

        public bool Initialized
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

        private Discord.Discord _discord;
        public  Discord.Discord Discord => _discord;

        #region Manager Caches

        private VoiceManager _voiceManager = null;

        public VoiceManager VoiceManager
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

        private LobbyManager _lobbyManager = null;

        public LobbyManager LobbyManager
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

        private OverlayManager _overlayManager = null;

        public OverlayManager OverlayManager
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

        #endregion

        #region Methods

        #region Lifetime

        public static DiscordApp InitializeApp(long clientId, ulong flags, int instanceId = 0)
        {
            return new DiscordApp(clientId, flags, instanceId);
        }

        public void Update()
        {
            _discord.RunCallbacks();
        }

        public void DestroyApp()
        {
            Distributor.UnsubscribeGameSDK();
            _discord.Dispose();
            Initialized = false;
        }

        #endregion

        #endregion

        #region Event Distributor Class Def

        private class EventDistributor
        {
            public readonly DiscordApp App;
            
            #region Variables

            //todo -- ensure hash set iteration doesn't generate too much garbage
            private HashSet<IGroupCallbacks> _groupListeners = new HashSet<IGroupCallbacks>();

            private HashSet<IVoiceCallbacks> _voiceListeners = new HashSet<IVoiceCallbacks>();

            private HashSet<IOverlayCallbacks> _overlayListeners = new HashSet<IOverlayCallbacks>();

            #endregion

            public EventDistributor(DiscordApp app)
            {
                Assert.IsNotNull(app);
                App = app;
            }

            #region Register To Managers

            public void UnsubscribeGameSDK()
            {
                Assert.IsNotNull(App.Discord);
                UnsubscribeVoice();
                UnsubscribeOverlay();
                UnsubscribeLobbyGroup();
            }

            public void UnsubscribeVoice()
            {
                VoiceManager voice = App.VoiceManager;
                voice.OnSettingsUpdate -= OnSettingsUpdateInvoke;
            }

            public void UnsubscribeOverlay()
            {
                OverlayManager overlay = App.OverlayManager;
                overlay.OnToggle -= OnToggleInvoke;
            }

            public void UnsubscribeLobbyGroup()
            {
                LobbyManager lobby = App.LobbyManager;
                lobby.OnLobbyDelete      -= InvokeOnLobbyDelete;
                lobby.OnLobbyMessage     -= InvokeOnLobbyMessage;
                lobby.OnLobbyUpdate      -= InvokeOnLobbyUpdate;
                lobby.OnSpeaking         -= InvokeOnSpeaking;
                lobby.OnMemberConnect    -= InvokeOnMemberConnect;
                lobby.OnMemberDisconnect -= InvokeOnMemberDisconnect;
                lobby.OnMemberUpdate     -= InvokeOnMemberUpdate;
                lobby.OnNetworkMessage   -= InvokeOnNetworkMessage;
            }


            public void SubscribeGameSDK()
            {
                UnsubscribeGameSDK();
                SubscribeVoice();
                SubscribeOverlay();
                SubscribeLobbyGroup();
            }

            public void SubscribeVoice()
            {
                VoiceManager voice = App.VoiceManager;
                voice.OnSettingsUpdate += OnSettingsUpdateInvoke;
            }

            public void SubscribeOverlay()
            {
                OverlayManager overlay = App.OverlayManager;
                overlay.OnToggle += OnToggleInvoke;
            }

            public void SubscribeLobbyGroup()
            {
                LobbyManager lobby = App.LobbyManager;
                lobby.OnLobbyDelete      += InvokeOnLobbyDelete;
                lobby.OnLobbyMessage     += InvokeOnLobbyMessage;
                lobby.OnLobbyUpdate      += InvokeOnLobbyUpdate;
                lobby.OnSpeaking         += InvokeOnSpeaking;
                lobby.OnMemberConnect    += InvokeOnMemberConnect;
                lobby.OnMemberDisconnect += InvokeOnMemberDisconnect;
                lobby.OnMemberUpdate     += InvokeOnMemberUpdate;
                lobby.OnNetworkMessage   += InvokeOnNetworkMessage;
            }
            #endregion

            #region Invoke Listeners

            private void OnSettingsUpdateInvoke()
            {
                VoiceManager voice = App.VoiceManager;
                foreach (var listener in _voiceListeners)
                {
                    listener.OnSettingsUpdate();
                }
            }

            private void OnToggleInvoke(bool locked)
            {
                OverlayManager manager = App.OverlayManager;
                foreach (var overlay in _overlayListeners)
                {
                    overlay.OnToggle(locked);
                }
            }
            
            private void InvokeOnNetworkMessage(long lobbyid, long userid, byte channelid, byte[] data)
            {
                LobbyManager manager = App.LobbyManager;
                foreach (var group in _groupListeners)
                {
                    group.OnNetworkMessage(lobbyid, userid, channelid, data);
                }
            }

            private void InvokeOnMemberDisconnect(long lobbyid, long userid)
            {
                LobbyManager manager = App.LobbyManager;
                foreach (var group in _groupListeners)
                {
                    group.OnMemberDisconnect(lobbyid, userid);
                }
            }

            private void InvokeOnSpeaking(long lobbyid, long userid, bool speaking)
            {
                LobbyManager manager = App.LobbyManager;
                foreach (var group in _groupListeners)
                {
                    group.OnSpeaking(lobbyid, userid, speaking);
                }
            }

            private void InvokeOnLobbyUpdate(long lobbyid)
            {
                LobbyManager manager = App.LobbyManager;
                foreach (var group in _groupListeners)
                {
                    group.OnLobbyUpdate(lobbyid);
                }
            }

            private void InvokeOnLobbyMessage(long lobbyid, long userid, byte[] data)
            {
                LobbyManager manager = App.LobbyManager;
                foreach (var group in _groupListeners)
                {
                    group.OnLobbyMessage(lobbyid, userid, data);
                }
            }
            
            private void InvokeOnMemberUpdate(long lobbyid, long userid)
            {
                LobbyManager manager = App.LobbyManager;
                foreach (var group in _groupListeners)
                {
                    group.OnMemberUpdate(lobbyid, userid);
                }
            }

            private void InvokeOnMemberConnect(long lobbyid, long userid)
            {
                LobbyManager manager = App.LobbyManager;
                foreach (var group in _groupListeners)
                {
                    group.OnMemberConnect(lobbyid, userid);
                }
            }

            private void InvokeOnLobbyDelete(long lobbyid, uint reason)
            {
                LobbyManager manager = App.LobbyManager;
                foreach (var group in _groupListeners)
                {
                    group.OnLobbyDelete(lobbyid, reason);
                }
            }

            #endregion

            #region Listener Registration

            public void RegisterCallbacks(IGroupCallbacks listener)
            {
                _groupListeners.Add(listener);
            }

            public void RegisterCallbacks(IVoiceCallbacks listener)
            {
                _voiceListeners.Add(listener);
            }

            public void RegisterCallbacks(IOverlayCallbacks listener)
            {
                _overlayListeners.Add(listener);
            }

            public void RemoveCallbacks(IGroupCallbacks listener)
            {
                _groupListeners.Remove(listener);
            }

            public void RemoveCallbacks(IVoiceCallbacks listener)
            {
                _voiceListeners.Remove(listener);
            }

            public void RemoveCallbacks(IOverlayCallbacks listener)
            {
                _overlayListeners.Remove(listener);
            }

            #endregion
        }
        
        #endregion

        #region IAppLayer

        #region Callback Registration

        public void RegisterCallbacks(IGroupCallbacks listener)
        {
            Distributor.RegisterCallbacks(listener);
        }

        public void RegisterCallbacks(IVoiceCallbacks listener)
        {
            Distributor.RegisterCallbacks(listener);
        }

        public void RegisterCallbacks(IOverlayCallbacks listener)
        {
            Distributor.RegisterCallbacks(listener);
        }

        public void RemoveCallbacks(IGroupCallbacks listener)
        {
            Distributor.RemoveCallbacks(listener);
        }

        public void RemoveCallbacks(IVoiceCallbacks listener)
        {
            Distributor.RemoveCallbacks(listener);
        }

        public void RemoveCallbacks(IOverlayCallbacks listener)
        {
            Distributor.RemoveCallbacks(listener);
        }

        #endregion

        #endregion

    }
}