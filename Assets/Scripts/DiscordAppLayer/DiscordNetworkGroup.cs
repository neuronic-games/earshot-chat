using System;
using System.Collections.Generic;
using System.Text;
using AppLayer.Callbacks;
using AppLayer.NetworkGroups;
using AppLayer.Voice;
using Discord;
using UnityEngine;

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
        public readonly string     Secret;

        public uint Capacity { get; protected set; }
        public bool Locked   { get; protected set; }
        public long OwnerId  { get; protected set; }

        public DiscordNetworkGroup(DiscordApp app, ref Lobby lobby)
        {
            App      = app;
            LobbyId  = lobby.Id;
            Secret   = lobby.Secret;
            Capacity = lobby.Capacity;
            Locked   = lobby.Locked;
            OwnerId  = lobby.OwnerId;
            IsAlive  = true;

            _channel = new VoiceChannel(InvokeDisconnect);

            Subscribe();
            
            UpdateCustomProperties();
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

        public bool IsConnectedVoice => IsAlive && _channel.IsConnected;

        public void ConnectVoice(Action onSuccess, Action onFail)
        {
            if (IsConnectedVoice || _channel.IsConnecting) return;
            _channel.ConnectingState();
            _channel.OnConnectSuccess = onSuccess;
            _channel.OnConnectFail    = onFail;
            var lobby = App.LobbyManager;
            lobby.ConnectVoice(LobbyId, _channel.OnVoiceConnect);
        }

        public void DisconnectVoice()
        {
            if (!IsConnectedVoice) return;
            if (_channel.IsConnecting)
            {
                _channel.DisconnectedState();
                return;
            }

            var lobby = App.LobbyManager;
            lobby.DisconnectVoice(LobbyId, _channel.OnVoiceDisconnect);
        }

        public event Action OnDisconnectVoice;

        private void InvokeDisconnect()
        {
            OnDisconnectVoice?.Invoke();
        }

        #endregion

        #region INetworkGroup

        #region Properties

        private List<DiscordUser> _members = new List<DiscordUser>();

        public (string GroupId, string JoinPassword) IdAndPassword => (LobbyId.ToString(), Secret);
        public bool IsAlive { get; protected set; }
        public IUser LocalUser => LocalDiscordUser;
        public DiscordUser LocalDiscordUser => _members.Find(m => m.DiscordUserId == App.LocalUser.Id);
        public IReadOnlyList<IUser> Members => _members;
        public bool IsOwner => App.LocalUser.Id == OwnerId;

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
            set => SetCustomProperties(value, null);
        }

        public void SetCustomProperties(IReadOnlyDictionary<string, string> properties, Action onImplemented)
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
                if (result != Result.Ok)
                {
                    Debug.Log($"Failed to set properties for lobby {LobbyId}.");
                }

                onImplemented?.Invoke();
            });
        }

        public void DeleteCustomProperties(IReadOnlyList<string> properties, Action onImplemented)
        {
            LobbyManager     lobby  = App.LobbyManager;
            LobbyTransaction update = lobby.GetLobbyUpdateTransaction(LobbyId);
            for (var i = 0; i < properties.Count; i++)
            {
                update.DeleteMetadata(properties[i]);
            }

            lobby.UpdateLobby(LobbyId, update, result =>
            {
                if (result != Result.Ok)
                {
                    Debug.Log($"Failed to delete properties for lobby {LobbyId}.");
                }

                onImplemented?.Invoke();
            });
        }

        public void SetOrDeleteCustomProperty(string key, string value)
        {
            LobbyManager     lobby  = App.LobbyManager;
            LobbyTransaction update = lobby.GetLobbyUpdateTransaction(LobbyId);
            if (string.IsNullOrEmpty(value))
            {
                update.DeleteMetadata(key);
            }
            else
            {
                update.SetMetadata(key, value);
            }

            lobby.UpdateLobby(LobbyId, update, (result =>
            {
                if (result != Result.Ok)
                {
                    Debug.Log($"Update lobby failed {result}.");
                }
            }));
        }
        
        public void UpdateCustomProperties()
        {
            _customProperties.Clear();

            var manager   = App.LobbyManager;
            int metaCount = manager.LobbyMetadataCount(LobbyId);

            for (int i = 0; i < metaCount; i++)
            {
                string key   = manager.GetLobbyMetadataKey(LobbyId, i);
                string value = manager.GetLobbyMetadataValue(LobbyId, key);

                _customProperties[key] = value;
            }

            OnGroupPropertiesUpdated?.Invoke();
        }

        public void LeaveOrDestroy(Action<bool> onLeft)
        {
            if(IsConnectedVoice) DisconnectVoice();
            
            var manager = App.LobbyManager;
            if (OwnerId == App.LocalUser.Id)
            {
                //destroy
                manager.DeleteLobby(LobbyId, result => { onLeft?.Invoke(result == Result.Ok); });
            }
            else
            {
                manager.DisconnectLobby(LobbyId, result => { onLeft?.Invoke(result == Result.Ok); });
            }
        }

        #endregion

        #region Events

        public event Action OnDestroyed;
        public event Action OnUsersUpdated;
        public event Action OnGroupPropertiesUpdated;

        #endregion

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
            UpdateCustomProperties();
        }

        public void OnNetworkMessage(long lobbyid, long userid, byte channelid, byte[] data)
        {
            if (lobbyid != LobbyId) return;
            //todo network message
        }

        public void OnMemberConnect(long lobbyid, long userid)
        {
            if (lobbyid != LobbyId) return;
            var manager = App.LobbyManager;
            var user    = manager.GetMemberUser(LobbyId, userid);
            AddMember(user);
            OnUsersUpdated?.Invoke();
        }

        public void OnSpeaking(long lobbyid, long userid, bool speaking)
        {
            if (lobbyid != LobbyId) return;
            for (var i = 0; i < Members.Count; i++)
            {
                var member = _members[i];
                if (member.DiscordUserId == userid)
                {
                    member.Speaking(speaking);
                }
            }
        }

        public void OnMemberUpdate(long lobbyid, long userid)
        {
            if (lobbyid != LobbyId) return;

            var member = _members.Find(m => m.DiscordUserId == userid);
            if (member == null)
            {
                var manager = App.LobbyManager;
                manager.GetMemberUser(lobbyid, userid);
                
            }
            member.UpdateCustomProperties();
            OnUsersUpdated?.Invoke();
        }

        public void OnMemberDisconnect(long lobbyid, long userid)
        {
            if (lobbyid != LobbyId) return;
            var member = _members.Find(m => m.DiscordUserId == userid);
            _members.Remove(member);
            OnUsersUpdated?.Invoke();
        }

        public void OnLobbyDelete(long lobbyid, uint reason)
        {
            if (lobbyid != LobbyId) return;
            App.DeleteGroup(this);
            for (var i = 0; i < _members.Count; i++)
            {
                DiscordUser member = _members[i];
                RemoveMember(member);
            }

            _members.Clear();
            
            IsAlive = false;
            OnDestroyed?.Invoke();
        }

        public void OnLobbyMessage(long lobbyid, long userid, byte[] data)
        {
            if (lobbyid != LobbyId) return;
            //todo
        }

        #endregion

        #region Discord Group Methods

        public DiscordUser AddMember(User user)
        {
            var discordUser = _members.Find(disc => disc.DiscordUserId == user.Id);
            if (discordUser != null) return discordUser;
            discordUser = new DiscordUser(user.Id, 0, user.Username, App, this);
            _members.Add(discordUser);
            App.AddUser(discordUser); //todo -- handle in constructor
            return discordUser;
        }

        public void RemoveMember(DiscordUser member)
        {
            _members.Remove(member);
            App.DeleteUser(member);
        }

        #endregion

        #region Misc Methods

        private StringBuilder _sb = new StringBuilder();

        public override string ToString()
        {
            _sb.Clear();
            _sb.AppendLine($"GroupId: {LobbyId} Secret: {Secret}");
            _sb.AppendLine($"OwnerId: {OwnerId}");
            _sb.AppendLine($"Capacity: {Capacity}\tLocked: {Locked}");
            _sb.AppendLine($"Custom Properties:-");
            foreach (var kvp in _customProperties)
            {
                _sb.AppendLine($"\t{kvp.Key} : {kvp.Value}");
            }

            _sb.AppendLine($"Members:-");
            foreach (var discordUser in _members)
            {
                _sb.AppendLine($"{discordUser}");
            }

            return _sb.ToString();
        }

        #endregion
    }
}