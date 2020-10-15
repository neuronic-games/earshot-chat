using AppLayer.NetworkGroups;
using UnityEngine;
using Whoo.Views;

namespace Whoo
{
    public class Room : MonoBehaviour
    {
        #region Serialized

        [SerializeField]
        private RoomUsersView roomUsers = null;

        #endregion

        public virtual INetworkGroup Group { get; protected set; }

        public void Setup(INetworkGroup roomGroup)
        {
            DetachListeners();
            Group = roomGroup;
            AttachListeners();
            
            OnUsersUpdated();
            OnGroupPropertiesUpdated();
        }

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
            FindObjectOfType<FirstBuild>().ToStartScreen();
        }

        protected virtual void OnUsersUpdated()
        {
            Debug.Log($"{Group.Members.Count}");
            roomUsers.KeepOnly(Group.Members);
        }

        protected virtual void OnGroupPropertiesUpdated()
        {
            //todo
        }
    }
}