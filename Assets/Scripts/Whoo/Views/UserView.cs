using AppLayer.NetworkGroups;
using TMPro;
using UnityEngine;

namespace Whoo.Views
{
    public abstract class UserView : MonoBehaviour
    {
        [SerializeField]
        protected TextMeshProUGUI displayName;

        public IUser User { get; protected set; }

        public void RegisterUser(IUser user)
        {
            DetachListeners();
            this.User = user;
            AttachListeners();
            Refresh();
        }

        protected bool DetachListeners()
        {
            if (User == null) return false;
            User.OnSpeaking  -= Speaking;
            return true;
        }

        protected bool AttachListeners()
        {
            if (User == null) return false;
            User.OnSpeaking  += Speaking;
            return true;
        }

        public virtual void Refresh()
        {
            displayName.text = User.Name;
        }

        protected abstract void Speaking(bool speaking);
    }
}