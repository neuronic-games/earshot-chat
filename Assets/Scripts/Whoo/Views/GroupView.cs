using AppLayer.NetworkGroups;
using DiscordAppLayer;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Whoo.Views
{
    public abstract class GroupView : MonoBehaviour, IPointerDownHandler
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

        #region IPointerDownHandler

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                LeftClicked(eventData);
            }
            else if(eventData.button == PointerEventData.InputButton.Middle)
            {
                MiddleClicked(eventData);
            }
            else
            {
                RightClicked(eventData);
            }
        }

        protected abstract void MiddleClicked(PointerEventData eventData);

        protected abstract void RightClicked(PointerEventData eventData);

        protected abstract void LeftClicked(PointerEventData eventData);

        #endregion
    }
}