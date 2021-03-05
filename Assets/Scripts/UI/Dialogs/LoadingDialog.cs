using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR

#endif

namespace UI.Dialogs
{
    public class LoadingDialog : MonoBehaviour, IDialog<LoadingDialog.Settings>
    {
        #region Serialized

        [Header("References")]
        [SerializeField]
        private TextMeshProUGUI title;

        [SerializeField]
        private TextMeshProUGUI message;

        [SerializeField]
        private TextMeshProUGUI status;

        #endregion

        #region Class Definitions

        public struct Settings : IDialogSettings
        {
            public string Title;
            public string Message;
            public string Status;
        }

        #endregion


        #region Editor

#if UNITY_EDITOR

        [MenuItem("GameObject/UI/Dialogs/Loading Dialog")]
        private static void CreateInfoDialog()
        {
            var root = new GameObject(nameof(LoadingDialog), new[] {typeof(CanvasRenderer), typeof(RectTransform)});
            root.transform.SetParent(UnityEditor.Selection.activeGameObject.transform);
            UnityDialogController.AnchorFull(root.GetComponent<RectTransform>());
            var dialog   = root.AddComponent<LoadingDialog>();
            var darken   = UnityDialogController.Panel("Darkening",  root.transform);
            var dialogbg = UnityDialogController.Panel("Background", darken.transform);
            dialog.title   = UnityDialogController.Text(nameof(title),   dialogbg.transform);
            dialog.message = UnityDialogController.Text(nameof(message), dialogbg.transform);
            dialog.status  = UnityDialogController.Text(nameof(status),  dialogbg.transform);
        }

#endif

        #endregion

        #region IDialog

        public Action MainAction => null;

        public bool IsDisplayed { get; protected set; }

        public void Display(Dictionary<string, object> values)
        {
            Settings settings = new Settings();
            settings.Title   = Get<string>(nameof(Settings.Title));
            settings.Message = Get<string>(nameof(Settings.Message));
            Display(ref settings);

            T Get<T>(string key)
            {
                if (values.TryGetValue(key, out object value) && value is T Value) return Value;
                return default(T);
            }
        }

        public void Display(ref Settings settings)
        {
            gameObject.SetActive(true);
            _queue++;

            RefreshUI(ref settings);

            IsDisplayed = true;

            void RefreshUI(ref Settings s)
            {
                title.text   = s.Title;
                message.text = s.Message;
                status.text  = s.Status;
            }
        }

        public void Hide()
        {
            _queue      = Math.Max(0, _queue - 1);
            if (_queue == 0)
            {
                gameObject.SetActive(false);
                IsDisplayed = false;
            }
        }

        public void Setup()
        {
            Hide();
        }

        #endregion

        public void UpdateStatus(string stat)
        {
            status.text = stat;
        }

        private int _queue = 0;
    }
}