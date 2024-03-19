using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omnix.Notification
{
    [Serializable]
    public class ButtonAndText
    {
        [SerializeField, CanBeNull] private Button _button;
        [SerializeField, CanBeNull] private TextMeshProUGUI _text;
        [SerializeField] private string _defaultText;

        public void Set(IButtonSettings settings)
        {
            if (_button)
            {
                _button.gameObject.SetActive(settings.IsActive);
                _button.onClick.AddListener(settings.Callback);
            }

            if (_text)
            {
                _text.gameObject.SetActive(settings.IsActive);
                _text.SetText(string.IsNullOrEmpty(settings.Text) ? _defaultText : settings.Text);
            }
        }

        public void AddListener(UnityAction action)
        {
            if (_button)
            {
                _button.onClick.AddListener(action);
            }
        }
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ButtonAndText))]
    public class ButtonAndTextPropertyDrawer : PropertyDrawer
    {
        private GUIContent _autoDetectContent;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (property.isExpanded) return EditorGUIUtility.singleLineHeight * 4f;
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (_autoDetectContent == null)
            {
                _autoDetectContent = new GUIContent("Auto Detect");
            }
            
            EditorGUI.BeginProperty(position, label, property);

            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            SerializedProperty buttonProp = property.FindPropertyRelative("_button");
            SerializedProperty textProp = null;
            SerializedProperty defaultTextProp = null;
            
            
            Rect rect = new Rect(position);
            rect.x += EditorGUIUtility.labelWidth + 3f;
            rect.width -= EditorGUIUtility.labelWidth + 3f;
            EditorGUI.BeginChangeCheck();
            Button button = (Button)EditorGUI.ObjectField(rect, GUIContent.none, buttonProp.objectReferenceValue, typeof(Button), true);
            bool changed = EditorGUI.EndChangeCheck();
            if (changed)
            {
                textProp = property.FindPropertyRelative("_text");
                defaultTextProp = property.FindPropertyRelative("_defaultText");
                UpdateFields(button, buttonProp, textProp, defaultTextProp);
            }

            if (!property.isExpanded) return;
            if (!changed)
            {
                textProp = property.FindPropertyRelative("_text");
                defaultTextProp = property.FindPropertyRelative("_defaultText");
            }
            
           
            
            
            position.x += EditorGUIUtility.singleLineHeight;
            position.width -= EditorGUIUtility.singleLineHeight;
            position.y += position.height;
            EditorGUI.PropertyField(position, buttonProp);
            position.y += position.height;
            EditorGUI.PropertyField(position, textProp);
            position.y += position.height;
            EditorGUI.PropertyField(position, defaultTextProp);

        }

        private void UpdateFields(Button button, SerializedProperty buttonProp, SerializedProperty textProp, SerializedProperty defaultTextProp)
        {
            if (button == null) return;

            buttonProp.objectReferenceValue = button;
            TextMeshProUGUI text = button.transform.GetComponentInChildren<TextMeshProUGUI>();
            if (text)
            {
                textProp.objectReferenceValue = text;
                defaultTextProp.stringValue = text.text;
            }
        }
    }
    #endif
}