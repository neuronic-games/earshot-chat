using System;
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
            public                  INetworkGroup              Group           => null;
            public                  bool                       IsReady         => false;
            public                  string                     Name            => string.Empty;
            private static readonly Dictionary<string, string> s_CustomProperties = new Dictionary<string, string>();

            public IReadOnlyDictionary<string, string> CustomProperties
            {
                get => s_CustomProperties;
                set => SetCustomProperties(value, null);
            }

            public void SetCustomProperties(IReadOnlyDictionary<string, string> value, Action onImplemented)
            {
                onImplemented?.Invoke();
            }

            public bool Equals(IUser other)
            {
                return other is NullUser;
            }

            public void DeleteCustomProperties(IReadOnlyList<string> properties, Action onImplemented)
            {
                onImplemented?.Invoke();
            }

            public void SetOrDeleteCustomProperty(string key, string value)
            {
                
            }

            public event Action<bool> OnSpeaking;

            public event Action OnCustomPropertiesUpdated;

            public void Speaking()
            {
                OnSpeaking?.Invoke(false);
            }
        }

        public IUser                        LocalUser   { get; } = new NullUser();
        public IReadOnlyList<IUser>         KnownUsers  { get; } = new List<IUser>();
        public IReadOnlyList<INetworkGroup> KnownGroups { get; } = new List<INetworkGroup>();

        #endregion

        #region Factory

        public int  GroupCapacity  => 0;
        public bool CanCreateGroup => false;

        public bool CanJoinGroup => false;

        public void CreateNewGroup(uint capacity, bool locked, Action<INetworkGroup> onCreated)
        {
            onCreated?.Invoke(null);
        }

        public void JoinGroup(long groupId, string secret, Action<INetworkGroup> onJoined)
        {
            onJoined?.Invoke(null);
        }

        public void DeleteGroup(INetworkGroup group)
        {
        }
        
        

        #endregion
    }
}