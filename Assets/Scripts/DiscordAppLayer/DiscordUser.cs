using System;
using System.Collections.Generic;
using System.Text;
using AppLayer.NetworkGroups;
using Discord;
using UnityEngine;

namespace DiscordAppLayer
{
    public class DiscordUser : IUser, IEquatable<DiscordUser>
    {
        #region IUser

        public INetworkGroup Group
        {
            get => DiscordGroup;
        }

        public readonly DiscordApp          App;
        public          DiscordNetworkGroup DiscordGroup    { get; protected set; }
        public          bool                IsReady         { get; protected set; }
        public          int                 PermissionLevel { get; protected set; }
        public          string              UniqueId        { get; protected set; }
        public          string              Name            { get; protected set; }

        protected readonly Dictionary<string, string> _customProperties = new Dictionary<string, string>();

        public IReadOnlyDictionary<string, string> CustomProperties
        {
            get => _customProperties;
            set => SetCustomProperties(value, null);
        }

        public void SetCustomProperties(IReadOnlyDictionary<string, string> properties, Action onImplemented)
        {
            if (!DiscordApp.GetDiscordApp(out DiscordApp discord)) return;
            var manager     = discord.LobbyManager;
            var transaction = manager.GetMemberUpdateTransaction(DiscordGroup.LobbyId, DiscordUserId);
            foreach (var property in properties)
            {
                transaction.SetMetadata(property.Key, property.Value);
            }

            manager.UpdateMember(DiscordGroup.LobbyId, DiscordUserId, transaction, OnPropertiesSet);

            void OnPropertiesSet(Result result)
            {
                if (result != Result.Ok)
                {
                    Debug.Log(
                        $"Failed to set properties on {DiscordUserId} of group {DiscordGroup.LobbyId}. Result: {result}");
                }
                else
                {
                    Debug.Log($"Succeeded to set properties on {DiscordUserId} of group {DiscordGroup.LobbyId}.");
                    UpdateCustomProperties();
                }

                onImplemented?.Invoke();
            }
        }

        public void DeleteCustomProperties(IReadOnlyList<string> properties, Action onImplemented)
        {
            if (!DiscordApp.GetDiscordApp(out DiscordApp discord)) return;
            var manager     = discord.LobbyManager;
            var transaction = manager.GetMemberUpdateTransaction(DiscordGroup.LobbyId, DiscordUserId);
            for (var i = 0; i < properties.Count; i++)
            {
                transaction.DeleteMetadata(properties[i]);
            }

            manager.UpdateMember(DiscordGroup.LobbyId, DiscordUserId, transaction, OnPropertiesSet);

            void OnPropertiesSet(Result result)
            {
                if (result != Result.Ok)
                {
                    Debug.Log(
                        $"Failed to set properties on {DiscordUserId} of group {DiscordGroup.LobbyId}. Result: {result}");
                }
                else
                {
                    Debug.Log($"Succeeded to set properties on {DiscordUserId} of group {DiscordGroup.LobbyId}.");
                    UpdateCustomProperties();
                }

                onImplemented?.Invoke();
            }
        }

        public void SetOrDeleteCustomProperty(string key, string value)
        {
            if (!DiscordApp.GetDiscordApp(out DiscordApp discord)) return;
            var manager     = discord.LobbyManager;
            var transaction = manager.GetMemberUpdateTransaction(DiscordGroup.LobbyId, DiscordUserId);
            if (string.IsNullOrEmpty(value))
            {
                transaction.DeleteMetadata(key);
            }
            else
            {
                transaction.SetMetadata(key, value);
            }

            manager.UpdateMember(DiscordGroup.LobbyId, DiscordUserId, transaction, OnPropertiesSet);

            void OnPropertiesSet(Result result)
            {
                if (result != Result.Ok)
                {
                    Debug.Log($"Failed to set property for user {Name}");
                }
            }
        }

        #endregion

        public long DiscordUserId { get; protected set; } = -1;

        #region Constructors and Assignment Methods

        public DiscordUser(DiscordApp app, DiscordNetworkGroup group)
        {
            IsReady      = false;
            App          = app;
            DiscordGroup = group;
        }

        public DiscordUser(long userId, int permLevel, string name, DiscordApp app, DiscordNetworkGroup group)
        {
            DiscordUserId   = userId;
            UniqueId = DiscordUserId.ToString();
            PermissionLevel = permLevel;
            Name            = name;
            App             = app;
            DiscordGroup    = group;
            IsReady         = true;
            OnReady();
        }

        public void SetUserIdName(long userId)
        {
            if (IsReady) return;
            DiscordUserId = userId;
            UniqueId = DiscordUserId.ToString();
            IsReady       = true;
            OnReady();
        }

        public void UpdateName(string name)
        {
            Name = name;
        }

        public void SetPermissionLevel(int level)
        {
            PermissionLevel = level;
        }

        private void OnReady()
        {
            UpdateDiscordUser();
        }

        #endregion

        #region IEquatable<DiscordUser>

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DiscordUser);
        }

        public bool Equals(IUser other)
        {
            return this.Equals(other as DiscordUser);
        }

        public bool Equals(DiscordUser other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (other == null) return false;
            if (other.DiscordUserId == DiscordUserId)
            {
                //for easier finding
                //if (other.Group == this.Group) return true;
                return true;
            }

            return false;
        }

        #endregion

        public void FillFromDiscordUser(ref User user)
        {
            Name = user.Username;
            if (DiscordUserId == -1)
            {
                SetUserIdName(user.Id);
            }

            UpdateCustomProperties();
        }

        public virtual void UpdateCustomProperties()
        {
            _customProperties.Clear();

            var manager   = App.LobbyManager;
            int metaCount = manager.MemberMetadataCount(DiscordGroup.LobbyId, DiscordUserId);

            for (int i = 0; i < metaCount; i++)
            {
                string key   = manager.GetMemberMetadataKey(DiscordGroup.LobbyId, DiscordUserId, i);
                string value = manager.GetMemberMetadataValue(DiscordGroup.LobbyId, DiscordUserId, key);

                _customProperties[key] = value;
            }

            OnCustomPropertiesUpdated?.Invoke();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"User ID: {DiscordUserId}");
            sb.AppendLine($"Name: {Name}\tPermission Level: {PermissionLevel}\tReady: {IsReady}");
            sb.AppendLine($"Current Custom Properties:-");
            foreach (var kvp in CustomProperties)
            {
                sb.AppendLine($"\t{kvp.Key} : {kvp.Value}");
            }

            return sb.ToString();
        }

        public void UpdateDiscordUser()
        {
            var manager = App.UserManager;
            manager.GetUser(DiscordUserId, (Result result, ref User user) =>
            {
                if (result == Result.Ok)
                {
                    FillFromDiscordUser(ref user);
                }
            });
        }

        public event Action<bool> OnSpeaking;
        public event Action       OnCustomPropertiesUpdated;

        public void Speaking(bool speaking)
        {
            OnSpeaking?.Invoke(speaking);
        }
    }
}