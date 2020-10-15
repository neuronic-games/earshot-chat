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
                    foreach (var kvp in properties)
                    {
                        _customProperties[kvp.Key] = kvp.Value; //todo fix race condition
                    }
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
                    foreach (var property in properties)
                    {
                        _customProperties.Remove(property);
                    }
                }

                onImplemented?.Invoke();
            }
        }

        #endregion

        public long DiscordUserId { get; protected set; } = -1;

        #region Constructors and Assignment Methods

        public DiscordUser(DiscordApp app, DiscordNetworkGroup group)
        {
            IsReady           = false;
            App               = app;
            DiscordGroup = group;
        }

        public DiscordUser(long userId, int permLevel, string name, DiscordApp app, DiscordNetworkGroup group)
        {
            DiscordUserId     = userId;
            PermissionLevel   = permLevel;
            Name              = name;
            App               = app;
            DiscordGroup = group;
            IsReady           = true;
            OnReady();
        }

        public void SetUserIdName(long userId)
        {
            if (IsReady) return;
            DiscordUserId = userId;
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

        #endregion

        #region IEquatable<DiscordUser>
        
        public bool Equals(IUser other)
        {
            return this.Equals(other as DiscordUser);
        }
        
        public bool Equals(DiscordUser other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (other               == null) return false;
            if (other.DiscordUserId == DiscordUserId)
            {
                if (other.Group == this.Group) return true; // even if both are null
                return false;
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

        private void OnReady()
        {
            UpdateDiscordUser();
        }

        public void UpdateDiscordUser()
        {
            var manager = App.UserManager;
            manager.GetUser(DiscordUserId, ((Result result, ref User user) =>
            {
                if (result == Result.Ok)
                {
                    FillFromDiscordUser(ref user);
                }
            }));
        }

        public event Action<bool> OnSpeaking;

        public void Speaking(bool speaking)
        {
            OnSpeaking?.Invoke(speaking);
        }
    }
}