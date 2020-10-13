using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs
{
    public interface IDialogSettings
    {
    }

    public interface IDialog
    {
        bool   IsDisplayed { get; }
        Action MainAction  { get; }
        void   Setup();
        void   Display(Dictionary<string, object> values);
        void   Hide();
    }

    public interface IDialog<T> : IDialog where T : struct, IDialogSettings
    {
        void Display(ref T settings);
    }

    public class UnityDialogController : MonoBehaviour, IDialogController
    {
        [SerializeField]
        private bool logCalls = false;

        [Header("References")]
        [SerializeField]
        private Canvas canvas;

        //todo -- self registering custom dialogs

        [SerializeField]
        private InfoDialog infoDialog;

        [SerializeField]
        private LoadingDialog loadingDialog;

        [SerializeField]
        private OptionsDialog optionsDialog;

        public void Awake()
        {
            if (logCalls)
            {
                Dialog.Set(new LogDialogController(this));
            }
            else
            {
                Dialog.Set(this);
            }

            infoDialog.Setup();
            loadingDialog.Setup();
            optionsDialog.Setup();
        }

        public void RequestInfo(string title, string message, DialogStyle dialogStyle, Action onClose)
        {
            var settings = new InfoDialog.Settings()
            {
                Title   = title,
                Message = message,
                OnClose = onClose
            };
            infoDialog.Display(ref settings);
        }

        public void RequestLoading(bool open, string title = "", string message = "", string status = "")
        {
            if (!open)
            {
                loadingDialog.Hide();
            }
            else
            {
                var settings = new LoadingDialog.Settings()
                {
                    Title   = title,
                    Message = message,
                    Status  = status
                };
                loadingDialog.Display(ref settings);
            }
        }

        public void UpdateLoading(string status)
        {
            if (loadingDialog.IsDisplayed)
            {
                loadingDialog.UpdateStatus(status);
            }
        }

        public void RequestOptions(string title, string message, Action onAccept = null, Action onDecline = null)
        {
            var settings = new OptionsDialog.Settings()
            {
                Title     = title,
                Message   = message,
                OnAccept  = onAccept,
                OnDecline = onDecline
            };
            optionsDialog.Display(ref settings);
        }

        public void RequestInput<T>(string title, Action<T> onInput, Action onClose) where T : DialogInput
        {
            throw new NotImplementedException();
        }

        #region UI Creators

#if UNITY_EDITOR

        public static TextMeshProUGUI Text(string name, Transform parent)
        {
            var text = new GameObject(name, new[] {typeof(RectTransform), typeof(TextMeshProUGUI)});
            text.transform.SetParent(parent);
            var comp = text.GetComponent<TextMeshProUGUI>();
            AnchorFull(comp.rectTransform);
            return comp;
        }

        public static Button Button(string name, Transform parent)
        {
            var root = new GameObject(name, new[] {typeof(RectTransform), typeof(Image), typeof(Button)});
            root.transform.SetParent(parent);

            var button = root.GetComponent<Button>();
            button.targetGraphic = button.GetComponent<Image>();
            var text = Text("Text (TMP)", root.transform);
            return button;
        }

        public static Image Panel(string name, Transform parent)
        {
            var root = new GameObject(name, new[] {typeof(RectTransform), typeof(Image)});
            root.transform.SetParent(parent);
            var image = root.GetComponent<Image>();
            AnchorFull(image.rectTransform);
            return image;
        }

        public static void AnchorFull(RectTransform rect)
        {
            rect.offsetMin        = rect.anchorMin = Vector2.zero;
            rect.offsetMax        = rect.anchorMax = Vector2.one;
            rect.anchoredPosition = Vector2.zero;
        }

        private string RandomString => UnityEngine.Random.Range(10000, 100000).ToString();

        [ContextMenu("Loading Add")]
        public void LoadingAdd()
        {
            RequestLoading(true, RandomString, RandomString, RandomString);
        }

        [ContextMenu("Loading Sub")]
        public void LoadingSub()
        {
            RequestLoading(false);
        }

        [ContextMenu("Info Add")]
        public void InfoAdd()
        {
            RequestInfo(RandomString, RandomString, DialogStyle.Info, () => Debug.Log($"Close."));
        }

        [ContextMenu("Options Add")]
        public void OptionsAdd()
        {
            RequestOptions(RandomString, RandomString, () => Debug.Log($"Accepted."), () => Debug.Log($"Declined."));
        }

#endif

        #endregion
    }
}