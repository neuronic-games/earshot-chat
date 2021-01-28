using System;

namespace Dialogs
{
    public interface IDialogController : IService
    {
        void RequestInfo(string title, string message, DialogStyle dialogStyle, Action onClose);

        void RequestLoading(bool open, string title = "", string message = "", string status = "");

        void UpdateLoading(string status);

        void RequestOptions(string title, string message, Action onAccept = null, Action onDecline = null);

        void RequestInput<T>(string title, Action<T> onInput, Action onClose) where T : DialogInput;
    }
    
    public enum DialogStyle
    {
        Info,
        Warning,
        Error
    }

    [Serializable]
    public class DialogInput
    {
    }
}