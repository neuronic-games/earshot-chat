using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif


namespace UI
{
    public interface ITextContext
    {
        string Format { get; set; }
        void   SetText(string text);
        string GetText(bool   formatted);
    }

    [Serializable]
    public class TextContext : ITextContext
    {
        [SerializeField]
        private string format = "{0}";

        [SerializeField]
        private ComponentMode mode = ComponentMode.TMPro;

        public ComponentMode Mode => mode;

        public enum ComponentMode
        {
            TMPro,
            UnityText
        }

        [SerializeField]
        private string lastValue = string.Empty;

        [SerializeField]
        private TextMeshProUGUI textMeshPro;

        [SerializeField]
        private Text unityText;

        public string Format
        {
            get => format;
            set => format = value;
        }

        public void SetText(string text)
        {
            lastValue = text;
            switch (mode)
            {
                case ComponentMode.TMPro:
                    textMeshPro.SetText(string.Format(format, text));
                    break;
                case ComponentMode.UnityText:
                    unityText.text = string.Format(format, text);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public string GetText(bool formatted = false)
        {
            if (!formatted) return lastValue;
            switch (mode)
            {
                case ComponentMode.TMPro:
                    return textMeshPro.text;
                case ComponentMode.UnityText:
                    return unityText.text;
                default: throw new ArgumentOutOfRangeException();
            }
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(TextContext))]
    public class TextContextDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 4.5f;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Rect r = position;
            r.height = position.height / 4.5f;

            EditorGUI.LabelField(r, label);
            r.y += r.height * 1.1f;

            int indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;

            EditorGUI.BeginProperty(position, label, property);

            var enumProp   = property.FindPropertyRelative("mode");
            var formatProp = property.FindPropertyRelative("format");
            var lValue     = property.FindPropertyRelative("lastValue");

            EditorGUI.PropertyField(r, formatProp);
            r.y += r.height * 1.1f;

            Rect half = r;
            half.width = r.width / 2.0f;
            EditorGUI.PropertyField(half, enumProp, GUIContent.none);
            half.x += half.width;
            r.y    += r.height * 1.1f;

            if (Enum.TryParse(enumProp.enumNames[enumProp.enumValueIndex], out TextContext.ComponentMode value))
            {
                if (value == TextContext.ComponentMode.UnityText)
                {
                    //EditorGUILayout.HelpBox("Unity Text is deprecated in favor of the much better Text Mesh Pro.",MessageType.Warning);
                    var unityTextProp = property.FindPropertyRelative("unityText");
                    EditorGUI.PropertyField(half, unityTextProp, GUIContent.none);
                }
                else
                {
                    var tmproProp = property.FindPropertyRelative("textMeshPro");
                    EditorGUI.PropertyField(half, tmproProp, GUIContent.none);
                }
            }

            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.PropertyField(r, lValue, GUIContent.none);
            }

            EditorGUI.EndProperty();

            EditorGUI.indentLevel = indent;
        }
    }

#endif
}