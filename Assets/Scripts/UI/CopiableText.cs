using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UI
{
    public class CopiableText : MonoBehaviour, IPointerDownHandler
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
                GUIUtility.systemCopyBuffer = textDisplay.text;
            }
            else
            {
                GUIUtility.systemCopyBuffer = _currentText;
            }
            onCopied?.Invoke();
        }
    }
}