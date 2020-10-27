using AppLayer.NetworkGroups;
using DiscordAppLayer;
using UnityEngine;
using UnityEngine.Assertions;

namespace Whoo.Views
{
    public class GroupView : MonoBehaviour
    {
        #region Serialized

        [SerializeField]
        private UserListView usersView = null;

        #endregion
        
        public virtual INetworkGroup Group { get; protected set; }

        #region Setup

        public void Setup(INetworkGroup group)
        {
            Group = group;
            
            DiscordNetworkGroup lobby = group as DiscordNetworkGroup;
            Assert.IsNotNull(lobby);
            
            AttachListeners();

            if (!lobby.IsConnectedVoice)
            {
                lobby.ConnectVoice(null, null);
                lobby.OnDisconnectVoice += VoiceDisconnected;
            }
            
            ForceRefresh();
        }

        public void Clear()
        {
            DetachListeners();
        }

        public virtual void ForceRefresh()
        {
            OnUsersUpdated();
            OnGroupPropertiesUpdated();
        }
        
        private void VoiceDisconnected()
        {
            if (Group is DiscordNetworkGroup lobby) lobby.OnDisconnectVoice -= VoiceDisconnected;
            
            Debug.Log($"Voice disconnected.");
        } 

        #region Listeners
        
        protected virtual bool DetachListeners()
        {
            if (Group == null) return false;
            Group.OnDestroyed              -= OnGroupDestroyed;
            Group.OnUsersUpdated           -= OnUsersUpdated;
            Group.OnGroupPropertiesUpdated -= OnGroupPropertiesUpdated;
            return true;
        }

        protected virtual bool AttachListeners()
        {
            if (!DetachListeners()) return false;
            Group.OnDestroyed              += OnGroupDestroyed;
            Group.OnUsersUpdated           += OnUsersUpdated;
            Group.OnGroupPropertiesUpdated += OnGroupPropertiesUpdated;
            return true;
        }

        protected virtual void OnGroupDestroyed()
        {
            Whoo.Build.RefreshRoomScreen();
        }

        protected virtual void OnUsersUpdated()
        {
            usersView.KeepOnly(Group.Members, UserIsSitting);

            bool UserIsSitting(IUser user)
            {
                return user.CustomProperties.ContainsKey(Constants.Sitting);
            }
        }

        protected virtual void OnGroupPropertiesUpdated()
        {
            //todo
        }
        
        #endregion

        #endregion
    }
}