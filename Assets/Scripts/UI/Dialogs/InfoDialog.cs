using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialogs
{
    public class InfoDialog : MonoBehaviour, IDialog<InfoDialog.Settings>
    {
        #region Serialized

        [Header("References")]
        [SerializeField]
        private TextMeshProUGUI title;

        [SerializeField]
        private TextMeshProUGUI message;

        [SerializeField]
        private Button closeButton;

        #endregion

        #region Class Definitions

        public struct Settings : IDialogSettings
        {
            public string Title;
            public string Message;
            public Action OnClose;
        }

        #endregion

        #region Editor

#if UNITY_EDITOR

        [UnityEditor.MenuItem("GameObject/UI/Dialogs/Info Dialog")]
        private static void CreateInfoDialog()
        {
            var root = new GameObject(nameof(InfoDialog), new[] {typeof(CanvasRenderer), typeof(RectTransform)});
            root.transform.SetParent(UnityEditor.Selection.activeGameObject.transform);
            UnityDialogController.AnchorFull(root.GetComponent<RectTransform>());
            var dialog = root.AddComponent<InfoDialog>();
            var darken = UnityDialogController.Panel("Darkening", root.transform);
            var dialogbg = UnityDialogController.Panel("Background", darken.transform);
            dialog.title       = UnityDialogController.Text(nameof(title), dialogbg.transform);
            dialog.message     = UnityDialogController.Text(nameof(message), dialogbg.transform);
            dialog.closeButton = UnityDialogController.Button(nameof(closeButton), dialogbg.transform);
        }

#endif

        #endregion

        #region IDialog

        public bool IsDisplayed { get; protected set; }

        public Action MainAction { get; protected set; }

        public void Display(Dictionary<string, object> values)
        {
            Settings settings = new Settings();
            settings.Title   = Get<string>(nameof(Settings.Title));
            settings.Message = Get<string>(nameof(Settings.Message));
            settings.OnClose = Get<Action>(nameof(Settings.OnClose));
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

            if (IsDisplayed)
            {
                _items.Add(settings);
                return;
            }

            RefreshUI(ref settings);

            IsDisplayed = true;
        }

        private void RefreshUI(ref Settings s)
        {
            title.text   = s.Title;
            message.text = s.Message;
            MainAction   = s.OnClose;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            IsDisplayed = false;
        }

        #endregion

        private void OnClose_UI()
        {
            MainAction?.Invoke();
            DisplayNextItem();
        }

        public void Setup()
        {
            closeButton.onClick.AddListener(OnClose_UI);
            Hide();
        }

        private readonly List<Settings> _items = new List<Settings>();

        private void DisplayNextItem()
        {
            Hide();
            if (_items.Count > 0)
            {
                var settings = _items[0];
                Display(ref settings);
                _items.RemoveAt(0);
            }
            else
            {
                var settings = new Settings()
                {
                    Message = string.Empty,
                    Title   = string.Empty,
                    OnClose = null
                };
                RefreshUI(ref settings);
            }
        }
    }
}