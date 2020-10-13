using System;
using System.Collections.Generic;
using System.Text;
using AppLayer.NetworkGroups;
using Discord;

namespace DiscordAppLayer
{
    public class DiscordUser : IUser, IEquatable<DiscordUser>
    {
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

        #region IUser

        public bool   IsReady         { get; protected set; }
        public int    PermissionLevel { get; protected set; }
        public string Name            { get; protected set; }

        private readonly Dictionary<string, string> _customProperties = new Dictionary<string, string>();

        public IReadOnlyDictionary<string, string> CustomProperties
        {
            get => _customProperties;
            set => SetCustomProperties(value);
        }

        public void SetCustomProperties(IReadOnlyDictionary<string, string> value)
        {
            //todo
            throw new System.NotImplementedException();
        }

        public void DeleteCustomProperties(IReadOnlyList<string> properties)
        {
            //todo
            throw new System.NotImplementedException();
        }

        #region IEquatable<IUser>

        public bool Equals(IUser other)
        {
            return this.Equals(other as DiscordUser);
        }

        #endregion

        #endregion

        public long DiscordUserId { get; protected set; }

        #region Constructors and Assignment Methods

        public DiscordUser()
        {
            IsReady = false;
        }

        public DiscordUser(long userId, int permLevel, string name)
        {
            DiscordUserId = userId;
            IsReady       = true;
        }

        public void SetUserIdName(long userId)
        {
            if (IsReady) return;
            DiscordUserId = userId;
            IsReady       = true;
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

        public bool Equals(DiscordUser other)
        {
            if (object.ReferenceEquals(this, other)) return true;
            if (other               == null) return false;
            if (other.DiscordUserId == DiscordUserId) return true;
            return false;
        }

        #endregion

        public void FillFromDiscordUser(ref User user)
        {
            Name = user.Username;
        }
    }
}