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

        public IUser User { get; protected set; }

        public virtual void RegisterUser(IUser user)
        {
            DetachListeners();
            this.User = user;
            AttachListeners();
            Refresh();
        }

        public virtual void DeleteView()
        {
            DetachListeners();
            Destroy(gameObject);
        }

        protected virtual bool DetachListeners()
        {
            if (User == null) return false;
            User.OnSpeaking                -= Speaking;
            User.OnCustomPropertiesUpdated -= CustomPropertiesUpdated;
            return true;
        }

        protected virtual void CustomPropertiesUpdated()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Updated User " + User.Name);

            foreach (var prop in User.CustomProperties)
            {
                sb.AppendLine($"\t{prop.Key} : {prop.Value}");
            }
            Debug.Log(sb.ToString());
        }

        protected virtual bool AttachListeners()
        {
            if (User == null) return false;
            User.OnSpeaking                += Speaking;
            User.OnCustomPropertiesUpdated += CustomPropertiesUpdated;
            return true;
        }

        public virtual void Refresh()
        {
            displayName.text = User.Name;
        }

        protected abstract void Speaking(bool speaking);
    }
}