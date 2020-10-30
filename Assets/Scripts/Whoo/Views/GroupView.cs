using AppLayer.NetworkGroups;
using DiscordAppLayer;
using UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Whoo.Views
{
    public abstract class GroupView : MonoBehaviour, IClickHandler
    {
        #region Serialized

        [SerializeField]
        private UserListView usersView = null;

        #endregion

        public INetworkGroup Group { get; protected set; }

        #region Setup

        public void Setup(INetworkGroup group)
        {
            Group = group;

            DiscordNetworkGroup lobby = group as DiscordNetworkGroup;
            Assert.IsNotNull(lobby);

            AttachListeners();

            lobby.OnDisconnectVoice += VoiceDisconnected;

            OnUsersUpdated();
            OnGroupPropertiesUpdated();
        }

        public void Clear()
        {
            DetachListeners();
            Group = null;
            Destroy(gameObject);
        }

        public virtual void Refresh()
        {
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
            usersView.KeepOnly(Group.Members);
            usersView.KeepOnly(UserIsSitting);

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

        #region IClickHandler

        public abstract void LeftClick();
        public abstract void RightClick();
        public abstract void MiddleClick();

        #endregion
    }
}