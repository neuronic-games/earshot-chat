using System.Collections.Generic;
using AppLayer.NetworkGroups;
using DiscordAppLayer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Whoo.Views
{
    public abstract class GroupView : MonoBehaviour, IUserListView, IPointerEnterHandler, IPointerExitHandler
    {
        #region Serialized

        [SerializeField]
        protected Transform usersContentContainer = null;

        [SerializeField]
        private Image hoverPreview = default;

        #endregion

        #region IPointerXyzHandler

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (hoverPreview != null) hoverPreview.gameObject.SetActive(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (hoverPreview != null) hoverPreview.gameObject.SetActive(false);
        }

        #endregion

        #region IUserListView

        public abstract string    ListUniqueId         { get; }
        public          Transform UserContentContainer => usersContentContainer;

        protected static List<IUserListView> UserListViews = new List<IUserListView>();

        #endregion

        public INetworkGroup Group { get; protected set; }

        #region Setup

        public void Setup(INetworkGroup group)
        {
            if (!UserListViews.Contains(this)) UserListViews.Add(this);
            DetachListeners();

            Group = group;

            if (AttachListeners())
            {
                if (group is DiscordNetworkGroup lobby)
                    lobby.OnDisconnectVoice += VoiceDisconnected;

                OnUsersUpdated();
                OnGroupPropertiesUpdated();
            }
        }

        public void Clear()
        {
            UserListViews.Remove(this);
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

        protected abstract void OnUsersUpdated();

        protected abstract void OnGroupPropertiesUpdated();

        #endregion

        #endregion
    }
}