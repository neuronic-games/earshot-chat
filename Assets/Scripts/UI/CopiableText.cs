using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Whoo;

namespace UI
{
    public class CopiableText : MonoBehaviour, IPointerDownHandler, ITextContext
    {
        [SerializeField]
        private TextMeshProUGUI textDisplay = null;

        [SerializeField]
        private string textFormat = "{0}";

        [SerializeField]
        private bool copyWholeFormattedText = false;

        [SerializeField]
        private UnityEvent onCopied = null;

        private string _currentText;

        public string Text
        {
            get => _currentText;
            set
            {
                _currentText     = value;
                textDisplay.text = string.Format(textFormat, _currentText);
            }
        }

        public void Awake()
        {
            Text = string.Empty;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (copyWholeFormattedText)
            {
                Utils.SetClipboardText(textDisplay.text);
            }
            else
            {
                Utils.SetClipboardText(_currentText);
            }

            onCopied?.Invoke();
        }

        #region ITextContext

        public string Format
        {
            get => textFormat;
            set => textFormat = value;
        }

        public void SetText(string text)
        {
            Text = text;
        }

        public string GetText(bool formatted)
        {
            if (!formatted)
            {
                return _currentText;
            }
            else
            {
                return textDisplay.text;
            }
        }

        #endregion
    }
}