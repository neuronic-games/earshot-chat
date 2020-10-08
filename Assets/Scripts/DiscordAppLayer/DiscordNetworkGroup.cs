using System;
using System.Collections.Generic;
using AppLayer.Callbacks;
using AppLayer.NetworkGroups;
using AppLayer.Voice;
using Discord;

namespace DiscordAppLayer
{
    /// <summary>
    /// Wrapper over Discord Lobbies, renamed to limit confusion.
    /// </summary>
    public class DiscordNetworkGroup : INetworkGroup, IGroupCallbacks, IVoiceChannel, IEquatable<DiscordNetworkGroup>
    {
        #region Constructor + Setup

        public readonly DiscordApp App;
        public readonly long       LobbyId;

        public DiscordNetworkGroup(DiscordApp app, long networkId)
        {
            App     = app;
            LobbyId = networkId;

            _channel = new VoiceChannel(InvokeDisconnect);
        }

        public void Subscribe()
        {
            App.RegisterCallbacks((IGroupCallbacks) this);
        }

        public void Unsubscribe()
        {
            App.RemoveCallbacks((IGroupCallbacks) this);
        }

        #endregion

        #region IVoiceChannel

        private class VoiceChannel
        {
            public bool   IsConnected { get; private set; }
            public Action OnConnectSuccess;
            public Action OnConnectFail;
            public bool   IsConnecting { get; private set; }

            private Action whenDisconnected;

            public VoiceChannel(Action whenDisconnectedNotifyParent)
            {
                whenDisconnected = whenDisconnectedNotifyParent;
            }

            public void DisconnectedState()
            {
                Action failed = OnConnectFail;
                OnConnectFail    = null;
                OnConnectSuccess = null;
                IsConnecting     = false;
                IsConnected      = false;
                failed?.Invoke();
                whenDisconnected?.Invoke();
            }

            public void ConnectingState()
            {
                IsConnecting = true;
            }

            public void ConnectedState()
            {
                Action succeeded = OnConnectSuccess;
                OnConnectFail    = null;
                OnConnectSuccess = null;
                IsConnecting     = false;
                IsConnected      = true;
                succeeded?.Invoke();
            }

            public void OnVoiceConnect(Result result)
            {
                if (!IsConnecting || result != Discord.Result.Ok)
                {
                    DisconnectedState();
                }
                else
                {
                    ConnectedState();
                }
            }

            public void OnVoiceDisconnect(Result result)
            {
                if (IsConnecting || result != Discord.Result.Ok)
                {
                    //empty 
                }
                else
                {
                    DisconnectedState();
                }
            }
        }

        private VoiceChannel _channel;

        public bool IsConnected => IsAlive && _channel.IsConnected;

        public void Connect(Action onSuccess, Action onFail)
        {
            if (IsConnected || _channel.IsConnecting) return;
            _channel.ConnectingState();
            _channel.OnConnectSuccess = onSuccess;
            _channel.OnConnectFail    = onFail;
            var lobby = App.LobbyManager;
            lobby.ConnectVoice(LobbyId, _channel.OnVoiceConnect);
        }

        public void Disconnect()
        {
            if (!IsConnected) return;
            if (_channel.IsConnecting)
            {
                _channel.DisconnectedState();
                return;
            }

            var lobby = App.LobbyManager;
            lobby.DisconnectVoice(LobbyId, _channel.OnVoiceDisconnect);
        }

        public event Action OnDisconnect;

        private void InvokeDisconnect()
        {
            OnDisconnect?.Invoke();
        }

        #endregion

        #region INetworkGroup

        private List<DiscordUser> _members = new List<DiscordUser>();

        public bool                 IsAlive { get; protected set; }
        public IReadOnlyList<IUser> Members => _members;

        public void Broadcast(byte[] message)
        {
            LobbyManager lobby = App.LobbyManager;
            lobby.SendLobbyMessage(LobbyId, message, OnMessageResult);

            void OnMessageResult(Result result)
            {
                //todo -- reliable messages, exponential back-off?
            }
        }

        public void Broadcast(string message)
        {
            LobbyManager lobby = App.LobbyManager;
            lobby.SendLobbyMessage(LobbyId, message, OnMessageResult);

            void OnMessageResult(Result result)
            {
                //todo -- reliable messages, exponential back-off?
            }
        }

        private Dictionary<string, string> _customProperties = new Dictionary<string, string>();

        public IReadOnlyDictionary<string, string> CustomProperties
        {
            get => _customProperties;
            set => SetCustomProperties(value);
        }

        public void SetCustomProperties(IReadOnlyDictionary<string, string> properties)
        {
            LobbyManager     lobby  = App.LobbyManager;
            LobbyTransaction update = lobby.GetLobbyUpdateTransaction(LobbyId);
            foreach (var prop in properties)
            {
                if (prop.Value == null) update.DeleteMetadata(prop.Key);
                else update.SetMetadata(prop.Key, prop.Value);
            }

            lobby.UpdateLobby(LobbyId, update, result =>
            {
                //todo -- success/failure callbacks?
            });
        }

        public void DeleteCustomProperties(IReadOnlyList<string> properties)
        {
            LobbyManager     lobby  = App.LobbyManager;
            LobbyTransaction update = lobby.GetLobbyUpdateTransaction(LobbyId);
            for (var i = 0; i < properties.Count; i++)
            {
                update.DeleteMetadata(properties[i]);
            }

            lobby.UpdateLobby(LobbyId, update, result =>
            {
                //todo -- success/failure callbacks?
            });
        }

        #endregion

        #region IEquatables

        public bool Equals(INetworkGroup other)
        {
            return this.Equals(other as DiscordNetworkGroup);
        }

        public bool Equals(DiscordNetworkGroup other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (other         == null) return false;
            if (other.LobbyId == this.LobbyId) return true;
            return false;
        }

        #endregion

        #region IGroupCallbacks

        public void OnLobbyUpdate(long lobbyid)
        {
            if (lobbyid != LobbyId) return;
        }

        public void OnNetworkMessage(long lobbyid, long userid, byte channelid, byte[] data)
        {
            if (lobbyid != LobbyId) return;
            throw new System.NotImplementedException();
        }

        public void OnMemberConnect(long lobbyid, long userid)
        {
            if (lobbyid != LobbyId) return;
            throw new System.NotImplementedException();
        }

        public void OnSpeaking(long lobbyid, long userid, bool speaking)
        {
            if (lobbyid != LobbyId) return;
            throw new System.NotImplementedException();
        }

        public void OnMemberUpdate(long lobbyid, long userid)
        {
            if (lobbyid != LobbyId) return;
            throw new System.NotImplementedException();
        }

        public void OnMemberDisconnect(long lobbyid, long userid)
        {
            if (lobbyid != LobbyId) return;
            throw new System.NotImplementedException();
        }

        public void OnLobbyDelete(long lobbyid, uint reason)
        {
            if (lobbyid != LobbyId) return;
            throw new System.NotImplementedException();
        }

        public void OnLobbyMessage(long lobbyid, long userid, byte[] data)
        {
            if (lobbyid != LobbyId) return;
            throw new System.NotImplementedException();
        }

        #endregion
    }
}