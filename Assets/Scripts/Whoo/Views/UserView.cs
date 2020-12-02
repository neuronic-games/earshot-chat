using System.Text;
using AppLayer.NetworkGroups;
using TMPro;
using UnityEngine;

namespace Whoo.Views
{
    public abstract class UserView : MonoBehaviour
    {
        [SerializeField]
        protected TextMeshProUGUI displayName;

        protected WhooRoom _room = null;
        public    IUser    User { get; protected set; }

        #region Registration

        public virtual void RegisterUser(IUser user, WhooRoom room)
        {
            _room = room;
            DetachListeners();
            this.User = user;
            AttachListeners();
            Refresh();
        }

        public virtual void DeleteView()
        {
            DetachListeners();
            _room = null;
            User  = null;
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

        public virtual void Refresh()
        {
            displayName.text = User.Name;
        }

        protected abstract void Speaking(bool speaking);
    }
}