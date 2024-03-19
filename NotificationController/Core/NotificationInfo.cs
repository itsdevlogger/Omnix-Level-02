using System;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Omnix.Notification
{
    [Serializable]
    public class NotificationInfo
    {
        public enum Type
        {
            Info = 0,
            Success = 1,
            Error = 2,
            Confirm = 3
        }

        public Type type;
        public string title;
        public string details;
        public ThemeIndex themeIndex = ThemeIndex.Default;
        public float autohideDuration;
        public ButtonSettingsSerializable okayButton = new ButtonSettingsSerializable() { Text = "Okay" };
        public ButtonSettingsSerializable noButton = new ButtonSettingsSerializable() { Text = "No" };
        public ButtonSettingsSerializable cancelButton = new ButtonSettingsSerializable() { Text = "Cancel" };
    }


    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(NotificationInfo))]
    public class NotificationInfoDrawer : PropertyDrawer
    {
        private float Height(SerializedProperty property, string name) => EditorGUI.GetPropertyHeight(property.FindPropertyRelative(name), true) + EditorGUIUtility.standardVerticalSpacing;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            float common = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 5f;
            common += Height(property, "okayButton") + Height(property, "cancelButton");

            SerializedProperty typeProp = property.FindPropertyRelative("type");
            NotificationInfo.Type type = (NotificationInfo.Type)typeProp.enumValueIndex;
            if (type == NotificationInfo.Type.Confirm) return common + Height(property, "noButton");
            return common + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            if (property.isExpanded == false)
            {
                EditorGUI.EndProperty();
                return;
            }

            position.width -= EditorGUIUtility.singleLineHeight;
            position.x += EditorGUIUtility.singleLineHeight;
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            SerializedProperty typeProp = property.FindPropertyRelative("type");
            EditorGUI.PropertyField(position, typeProp);
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("title"));
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("details"));
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("themeIndex"));
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;

            NotificationInfo.Type type = (NotificationInfo.Type)typeProp.enumValueIndex;
            if (type == NotificationInfo.Type.Confirm)
            {
                // Don't draw Autohide
                // Draw okayButton, noButton, cancelButton
                position = DrawProp(position, property, "okayButton");
                position = DrawProp(position, property, "noButton");
                DrawProp(position, property, "cancelButton");
            }
            else
            {
                // Draw Autohide
                // Draw okayButton, cancelButton
                EditorGUI.PropertyField(position, property.FindPropertyRelative("autohideDuration"));
                position.y += position.height;
                position = DrawProp(position, property, "okayButton");
                DrawProp(position, property, "cancelButton");
            }

            EditorGUI.EndProperty();
        }

        private Rect DrawProp(Rect position, SerializedProperty parentProp, string proName)
        {
            SerializedProperty property = parentProp.FindPropertyRelative(proName);
            position.height = EditorGUI.GetPropertyHeight(property, true);
            EditorGUI.PropertyField(position, property, true);
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            return position;
        }
    }
    #endif
}