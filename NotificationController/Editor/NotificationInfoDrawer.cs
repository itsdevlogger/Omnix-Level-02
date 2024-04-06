using UnityEngine;
using UnityEditor;

namespace Omnix.Notification.CustomEditor
{
     [CustomPropertyDrawer(typeof(NotificationInfo))]
    public class NotificationInfoDrawer : PropertyDrawer
    {
        private float Height(SerializedProperty property, string name) => EditorGUI.GetPropertyHeight(property.FindPropertyRelative(name), true) + EditorGUIUtility.standardVerticalSpacing;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;

            float common = (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 5f;
            common += EditorGUIUtility.singleLineHeight * 3f; // Details text area
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
            float totalWidth = position.width;
            float originalX = position.x;
            
            // type and title
            position.width = EditorGUIUtility.labelWidth - 5f;
            SerializedProperty typeProp = property.FindPropertyRelative("type");
            EditorGUI.PropertyField(position, typeProp, GUIContent.none);
            position.x += EditorGUIUtility.labelWidth + 2f;
            position.width = totalWidth - position.width - 7f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("title"), GUIContent.none);

            // details
            position.x = originalX;
            position.width = totalWidth;
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            SerializedProperty details = property.FindPropertyRelative("details");
            // EditorGUI.LabelField(position, details.displayName);
            // position.y += position.height;
            position.height = EditorGUIUtility.singleLineHeight * 4f;
            details.stringValue = EditorGUI.TextArea(position, details.stringValue);
            position.y += position.height + EditorGUIUtility.standardVerticalSpacing;
            position.height = EditorGUIUtility.singleLineHeight;
            
            // themeIndex
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
}