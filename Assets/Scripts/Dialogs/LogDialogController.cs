using System;
using UnityEngine;

namespace Dialogs
{
    public class LogDialogController : IDialogController
    {
        public readonly IDialogController Logged;

        private long _logId = 0;

        public string LogMark => $"[Log:{_logId++}]\n";

        public LogDialogController(IDialogController logged)
        {
            Logged = logged;
        }

        public void RequestInfo(string title, string message, DialogStyle dialogStyle, Action onClose)
        {
            Debug.Log($"{LogMark}{nameof(RequestInfo)}: {title}\n--{dialogStyle}\n--{message}.");
            Logged.RequestInfo(title, message, dialogStyle, onClose);
        }

        public void RequestLoading(bool open, string title = "", string message = "", string status = "")
        {
            Debug.Log($"{LogMark}{nameof(RequestLoading)}: {open}--{title}\n--{message}\n--{status}.");
            Logged.RequestLoading(open, title, message, status);
        }

        public void UpdateLoading(string status)
        {
            Debug.Log($"{LogMark}{nameof(UpdateLoading)}: {status}.");
            Logged.UpdateLoading(status);
        }

        public void RequestOptions(string title, string message, Action onAccept = null, Action onDecline = null)
        {
            string logMark = LogMark;
            Debug.Log($"{logMark}{nameof(RequestOptions)}: {title}--{message}\n");
            Logged.RequestOptions(title, message, () =>
            {
                Debug.Log($"{logMark}{nameof(RequestOptions)}: {title}--Accepted");
                onAccept?.Invoke();
            }, () =>
            {
                Debug.Log($"{logMark}{nameof(RequestOptions)}: {title}--Declined");
                onDecline?.Invoke();
            });
        }

        public void RequestInput<T>(string title, Action<T> onInput, Action onClose) where T : DialogInput
        {
            string logMark = LogMark;
            Debug.Log($"{logMark}{nameof(RequestInput)}.{nameof(T)}: {title}\n");
            Logged.RequestInput<T>(title, (result) =>
            {
                Debug.Log($"{logMark}{nameof(RequestInput)}.{nameof(T)}: {title}--Accepted");
                onInput?.Invoke(result);
            }, () =>
            {
                Debug.Log($"{logMark}{nameof(RequestInput)}.{nameof(T)}: {title}--Declined");
                onClose?.Invoke();
            });
        }
    }
}