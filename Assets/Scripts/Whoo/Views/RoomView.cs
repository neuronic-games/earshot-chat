using System;
using System.Collections.Generic;
using System.ComponentModel;
using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Whoo.Views
{
    public class RoomView : GroupView, IClickDownHandler
    {
        #region Serialized

        [SerializeField]
        UserView perUserView = default;

        #endregion

        #region Room Binding

        public Action userAdded;

        public WhooRoom WhooRoom { get; private set; }

        public void Setup(WhooRoom whooRoom)
        {
            Assert.IsNotNull(whooRoom);

            WhooRoom                 =  whooRoom;
            WhooRoom.PropertyChanged += OnRoomUpdated;
            base.Setup(whooRoom.RoomGroup);
        }

        private void OnRoomUpdated(object sender, PropertyChangedEventArgs e)
        {
            if (!(sender is WhooRoom room)) return;
            switch (e.PropertyName)
            {
                case nameof(WhooRoom.Tables):
                    break;
                case nameof(WhooRoom.StrapiRoom):
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Abstract Methods

        public override string ListUniqueId { get; }

        protected override void OnUsersUpdated()
        {
            KeepOnly(Group.Members);

            AssignUserViewsToProperContainers(WhooRoom.RoomGroup);
        }

        protected override void OnGroupPropertiesUpdated()
        {
            AssignUserViewsToProperContainers(WhooRoom.RoomGroup);
        }

        #endregion
        
        private void AssignUserViewsToProperContainers(INetworkGroup @group)
        {
            foreach (var (user, view) in _views)
            {
                bool      visible = true;
                Transform parent  = null;

                if (group.CustomProperties.TryGetValue(GroupProps.TableUserIsSittingAt(user.UniqueId),
                        out string zoneId) &&
                    !string.IsNullOrEmpty(zoneId))
                {
                    var userListView = UserListViews.Find(ulv => ulv.ListUniqueId == zoneId);
                    if (userListView != null)
                    {
                        parent = userListView.UserContentContainer.transform;
                    }
                    else
                    {
                        visible = false;
                    }
                }
                else
                {
                    parent = usersContentContainer;
                }

                if (view.gameObject.activeSelf != visible)
                    view.gameObject.SetActive(visible);
                if (view.transform.parent != parent)
                    view.transform.SetParent(parent);
            }
        }

        #region Users Views Handling

        private Dictionary<IUser, UserView> _views = new Dictionary<IUser, UserView>();

        public void AddView(IUser user)
        {
            var instance = Instantiate(perUserView, usersContentContainer);
            _views[user] = instance;
            userAdded?.Invoke();
            instance.RegisterUser(user, WhooRoom);
        }

        public void RemoveUser(IUser user)
        {
            if (_views.TryGetValue(user, out var view))
            {
                RemoveView(view);
                _views.Remove(user);
            }
        }

        private static void RemoveView(UserView view)
        {
            if (view != null) view.DeleteView();
        }

        private HashSet<IUser> _toKeep   = new HashSet<IUser>();
        private HashSet<IUser> _toRemove = new HashSet<IUser>();

        public void KeepOnly(IReadOnlyList<IUser> users)
        {
            _toKeep.Clear();
            _toRemove.Clear();

            for (var i = 0; i < users.Count; i++)
            {
                var user = users[i];
                _toKeep.Add(user);
                if (_views.TryGetValue(user, out var view))
                {
                    //view.Refresh();
                }
                else
                {
                    AddView(user);
                }
            }

            foreach (var kvp in _views)
            {
                if (_toKeep.Contains(kvp.Key)) continue;
                _toRemove.Add(kvp.Key);
            }

            foreach (var user in _toRemove)
            {
                RemoveUser(user);
            }
        }

        public void KeepOnly(Predicate<IUser> keep)
        {
            _toRemove.Clear();

            foreach (var kvp in _views)
            {
                if (keep.Invoke(kvp.Key)) continue;
                _toRemove.Add(kvp.Key);
            }

            foreach (var user in _toRemove)
            {
                RemoveUser(user);
            }

            _toRemove.Clear();
        }

        #endregion

        #region IPointerDownHandler

        public void MiddleClick(PointerEventData ptrData)
        {
            //ignore
        }

        public void RightClick(PointerEventData ptrData)
        {
            //ignore
        }

        public void LeftClick(PointerEventData ptrData)
        {
            if (WhooRoom.CurrentSitting == null) return;
            WhooRoom.SeatLocalUserAtTableAsync(null).Forget();
        }

        #endregion
    }
}