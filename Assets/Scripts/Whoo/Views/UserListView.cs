using System;
using System.Collections.Generic;
using AppLayer.NetworkGroups;
using UnityEngine;

namespace Whoo.Views
{
    //todo -- change to interface
    public class UserListView : MonoBehaviour
    {
        #region Serialized

        [SerializeField]
        private Transform contentContainer = null;

        [SerializeField]
        private UserView perUserView = null;
        
        #endregion
        
        private Dictionary<IUser, UserView> _views = new Dictionary<IUser, UserView>();

        public void AddView(IUser user)
        {
            var instance = Instantiate(perUserView, contentContainer);
            _views[user] = instance;
            instance.RegisterUser(user);
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

        private HashSet<IUser> _toKeep = new HashSet<IUser>();
        private HashSet<IUser> _toRemove = new HashSet<IUser>();
        
        public void KeepOnly(IReadOnlyList<IUser> users, Predicate<IUser> satisfies = null)
        {
            if (satisfies == null) satisfies = AlwaysTrue;
            for (var i = 0; i < users.Count; i++)
            {
                var user = users[i];
                if (!satisfies.Invoke(user)) continue;
                _toKeep.Add(user);
                if (_views.TryGetValue(user, out var view))
                {
                    view.Refresh();
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
            
            _toKeep.Clear();
            _toRemove.Clear();

        }
        bool AlwaysTrue(IUser u) => true;
    }
}