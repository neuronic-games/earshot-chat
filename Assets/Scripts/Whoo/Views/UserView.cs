using System.Text;
using AppLayer.NetworkGroups;
using Cysharp.Threading.Tasks;
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

        #region Registration

        public virtual void RegisterUser(IUser user, WhooRoom room)
        {
            Room = room;
            DetachListeners();
            this.User = user;
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