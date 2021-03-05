using System;

namespace UI.Dialogs
{
    public class NullDialogController : IDialogController
    {
        public void RequestInfo(string title, string message, DialogStyle dialogStyle, Action onClose)
        {
        }

        public void RequestLoading(bool open, string title = "", string message = "", string status = "")
        {
        }

        public void UpdateLoading(string status)
        {
        }

        public void RequestOptions(string title, string message, Action onAccept = null, Action onDecline = null)
        {
        }

        public void RequestInput<T>(string title, Action<T> onInput, Action onClose) where T : DialogInput
        {
        }
    }
}