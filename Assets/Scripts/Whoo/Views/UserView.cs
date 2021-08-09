using System;
using System.Collections.Generic;
using System.Text;
using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
using Discord;
using TMPro;
using UnityEngine;

namespace Whoo.Views
{
    public abstract class UserView : MonoBehaviour
    {
        [SerializeField]
        protected TextMeshProUGUI displayName;

        protected WhooRoom Room;
        public    IUser    User { get; protected set; }

        public class UsersInRoom
        {
            public IUser User;
            public WhooRoom Room;

            public void SetValues(IUser User, WhooRoom Room)
            {
                this.User = User;
                this.Room = Room;
            }
        }

        public List<UsersInRoom> usersInRoomList = new List<UsersInRoom>();

        #region Registration

        public virtual void RegisterUser(IUser user, WhooRoom room)
        {
            Room = room;
            DetachListeners();
            this.User = user;
            UsersInRoom usersInRoom = new UsersInRoom();
            usersInRoom.SetValues(user, room);
            usersInRoomList.Add(usersInRoom);
            AttachListeners();
            Refresh().Forget();
        }

        public virtual void DeleteView()
        {
            DetachListeners();
            Room = null;
            User = null;
            Destroy(gameObject);
        }

        protected virtual bool DetachListeners()
        {
            if (User == null) return false;
            User.OnSpeaking                -= Speaking;
            User.OnCustomPropertiesUpdated -= CustomPropertiesUpdated;
            return true;
        }

        protected virtual bool AttachListeners()
        {
            if (User == null) return false;
            User.OnSpeaking                += Speaking;
            User.OnCustomPropertiesUpdated += CustomPropertiesUpdated;
            return true;
        }

        #endregion

        protected virtual void CustomPropertiesUpdated()
        {
        }

        public virtual async UniTask Refresh()
        {
            displayName.text = User.Name;
            await UniTask.CompletedTask; //suppresses warning
        }

        protected abstract void Speaking(bool speaking);
    }
}