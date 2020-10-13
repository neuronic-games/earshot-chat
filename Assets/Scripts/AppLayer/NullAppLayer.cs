using System.Collections.Generic;
using AppLayer.Callbacks;
using AppLayer.NetworkGroups;

namespace AppLayer
{
    /// <summary>
    /// Null implementation of the app layer service. Provided by locator when another service isn't registered.
    /// </summary>
    public class NullAppLayer : IAppLayer
    {
        #region Registration

        public void RegisterCallbacks(IGroupCallbacks listener)
        {
        }

        public void RegisterCallbacks(IVoiceCallbacks listener)
        {
        }

        public void RegisterCallbacks(IOverlayCallbacks listener)
        {
        }

        public void RemoveCallbacks(IGroupCallbacks listener)
        {
        }

        public void RemoveCallbacks(IVoiceCallbacks listener)
        {
        }

        public void RemoveCallbacks(IOverlayCallbacks listener)
        {
        }

        public void RegisterCallbacks(IUserCallbacks listener)
        {
        }

        public void RemoveCallbacks(IUserCallbacks listener)
        {
        }

        #endregion

        public void DestroyApp()
        {
        }

        #region Management

        public class NullUser : IUser
        {
            public                  int                        PermissionLevel => 0;
            public                  bool                       IsReady         => false;
            public                  string                     Name            => string.Empty;
            private static readonly Dictionary<string, string> S_CustomProperties = new Dictionary<string, string>();

            public IReadOnlyDictionary<string, string> CustomProperties
            {
                get => S_CustomProperties;
                set => SetCustomProperties(value);
            }

            public void SetCustomProperties(IReadOnlyDictionary<string, string> value)
            {
            }

            public bool Equals(IUser other)
            {
                return other is NullUser;
            }

            public void DeleteCustomProperties(IReadOnlyList<string> properties)
            {
            }
        }

        public IUser                        LocalUser   { get; } = new NullUser();
        public IReadOnlyList<IUser>         KnownUsers  { get; } = new List<IUser>();
        public IReadOnlyList<INetworkGroup> KnownGroups { get; } = new List<INetworkGroup>();

        #endregion
    }
}