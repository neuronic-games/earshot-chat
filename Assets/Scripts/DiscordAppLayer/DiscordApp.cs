using System;
using System.Collections.Generic;
using AppLayer;
using AppLayer.Callbacks;
using AppLayer.NetworkGroups;
using Discord;
using UnityEngine.Assertions;

namespace DiscordAppLayer
{
    /// <summary>
    /// Discord-based implementation of the IAppLayer service. 
    /// </summary>
    public class DiscordApp : IAppLayer, IUserCallbacks
    {
        #region Constructor

        public DiscordApp(long clientId, ulong flags, int instanceId)
        {
            Environment.SetEnvironmentVariable("DISCORD_INSTANCE_ID", instanceId.ToString());
            _discord = new Discord.Discord(clientId, flags);

            Distributor = new EventDistributor(this);
            Distributor.RegisterCallbacks(this);
            Distributor.SubscribeGameSDK();

            Initialized = true;
        }

        #endregion

        //Contains fields, properties, events, etc.

        #region Data Members (Has Properties)

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
        
        private UserManager _userManager = null;

        public UserManager UserManager
        {
            get
            {
                Assert.IsTrue(Initialized);
                if (_userManager == null)
                {
                    _userManager = _discord.GetUserManager();
                }

                return _userManager;
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
            Distributor.RemoveCallbacks(this);
            Distributor.UnsubscribeGameSDK();
            _discord.Dispose();
            Initialized = false;
        }

        #endregion

        #endregion

        #region IUserCallbacks

        public void OnCurrentUserUpdate(IUser localUser)
        {
            if (!LocalUser.IsReady)
            {
                var user = UserManager.GetCurrentUser();
                _localUser.FillFromDiscordUser(ref user);
            }
        }

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
            
            private HashSet<IUserCallbacks> _userCallbacks = new HashSet<IUserCallbacks>();

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
                UnsubscribeUser();
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

            public void UnsubscribeUser()
            {
                UserManager user = App.UserManager;
                user.OnCurrentUserUpdate -= InvokeOnCurrentUserUpdate;
            }

            public void SubscribeGameSDK()
            {
                UnsubscribeGameSDK();
                SubscribeVoice();
                SubscribeOverlay();
                SubscribeLobbyGroup();
                SubscribeUser();
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

            public void SubscribeUser()
            {
                UserManager user = App.UserManager;
                user.OnCurrentUserUpdate += InvokeOnCurrentUserUpdate;
            }

            #endregion

            #region Invoke Listeners

            #region Voice
            private void OnSettingsUpdateInvoke()
            {
                VoiceManager voice = App.VoiceManager;
                foreach (var listener in _voiceListeners)
                {
                    listener.OnSettingsUpdate();
                }
            }
            #endregion

            #region Overlay
            private void OnToggleInvoke(bool locked)
            {
                OverlayManager manager = App.OverlayManager;
                foreach (var overlay in _overlayListeners)
                {
                    overlay.OnToggle(locked);
                }
            }
            #endregion

            #region Lobby Group
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
            
            #region User
            private void InvokeOnCurrentUserUpdate()
            {
                UserManager manager = App.UserManager;
                foreach (var listener in _userCallbacks)
                {
                    listener.OnCurrentUserUpdate(App.LocalUser);
                }
            }
            #endregion

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

            public void RegisterCallbacks(IUserCallbacks listener)
            {
                _userCallbacks.Add(listener);
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
            
            public void RemoveCallbacks(IUserCallbacks listener)
            {
                _userCallbacks.Remove(listener);
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

        #region Relationships

        private readonly DiscordUser               _localUser   = new DiscordUser();
        private readonly List<DiscordUser>         _knownUsers  = new List<DiscordUser>();
        private readonly List<DiscordNetworkGroup> _knownGroups = new List<DiscordNetworkGroup>();

        public IUser       LocalUser        => _localUser;
        public DiscordUser LocalDiscordUser => _localUser;

        public IReadOnlyList<IUser>       KnownUsers        => _knownUsers;
        public IReadOnlyList<DiscordUser> KnownDiscordUsers => _knownUsers;

        public IReadOnlyList<INetworkGroup> KnownGroups => _knownGroups;
        public IReadOnlyList<INetworkGroup> KnownDiscordGroups => _knownGroups;

        #endregion

        #endregion
    }
}