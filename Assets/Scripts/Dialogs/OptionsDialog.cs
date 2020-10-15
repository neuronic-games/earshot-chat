﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogs
{
    public class OptionsDialog : MonoBehaviour, IDialog<OptionsDialog.Settings>
    {
        #region Serialized

        [Header("References")]
        [SerializeField]
        private TextMeshProUGUI title;

        [SerializeField]
        private TextMeshProUGUI message;

        [SerializeField]
        private Button acceptButton;

        [SerializeField]
        private Button declineButton;

        #endregion

        #region Class Definitions

        public struct Settings : IDialogSettings
        {
            public string Title;
            public string Message;
            public Action OnAccept;
            public Action OnDecline;
        }

        #endregion


        #region Editor

#if UNITY_EDITOR

        [UnityEditor.MenuItem("GameObject/UI/Dialogs/Options Dialog")]
        private static void CreateInfoDialog()
        {
            var root = new GameObject(nameof(OptionsDialog), new[] {typeof(CanvasRenderer), typeof(RectTransform)});
            root.transform.SetParent(UnityEditor.Selection.activeGameObject.transform);
            UnityDialogController.AnchorFull(root.GetComponent<RectTransform>());
            var dialog   = root.AddComponent<OptionsDialog>();
            var darken   = UnityDialogController.Panel("Darkening",  root.transform);
            var dialogbg = UnityDialogController.Panel("Background", darken.transform);
            dialog.title         = UnityDialogController.Text(nameof(title), dialogbg.transform);
            dialog.message       = UnityDialogController.Text(nameof(message), dialogbg.transform);
            dialog.declineButton = UnityDialogController.Button(nameof(declineButton), dialogbg.transform);
            dialog.acceptButton  = UnityDialogController.Button(nameof(acceptButton), dialogbg.transform);
        }

#endif

        #endregion

        #region IDialog

        public bool IsDisplayed { get; protected set; }

        public Action MainAction
        {
            get => OnAccept;
        }

        public Action OnAccept  { get; protected set; }
        public Action OnDecline { get; protected set; }

        public void Display(Dictionary<string, object> values)
        {
            Settings settings = new Settings();
            settings.Title     = Get<string>(nameof(Settings.Title));
            settings.Message   = Get<string>(nameof(Settings.Message));
            settings.OnAccept  = Get<Action>(nameof(Settings.OnAccept));
            settings.OnDecline = Get<Action>(nameof(Settings.OnDecline));

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
            OnAccept     = s.OnAccept;
            OnDecline    = s.OnDecline;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            IsDisplayed = false;
        }

        #endregion

        private void OnAccept_UI()
        {
            OnAccept?.Invoke();
            DisplayNextItem();
        }

        private void OnDecline_UI()
        {
            OnDecline?.Invoke();
            DisplayNextItem();
        }

        public void Setup()
        {
            acceptButton.onClick.AddListener(OnAccept_UI);
            declineButton.onClick.AddListener(OnDecline_UI);
            Hide();
        }

        private readonly List<Settings> _items = new List<Settings>();

        private void DisplayNextItem()
        {
            Hide();
            if (_items.Count > 0)
            {
                var settings = _items[0];
                _items.RemoveAt(0);
                Display(ref settings);
            }
            else
            {
                var settings = new Settings()
                {
                    Message   = string.Empty,
                    Title     = string.Empty,
                    OnAccept  = null,
                    OnDecline = null
                };
                RefreshUI(ref settings);
            }
        }
    }
}