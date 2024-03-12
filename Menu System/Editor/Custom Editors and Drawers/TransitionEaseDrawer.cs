using MenuManagement.Base;
using UnityEditor;
using UnityEngine;

namespace MenuManagement.Editor
{
    [CustomPropertyDrawer(typeof(TransitionEase))]
    public class TransitionEaseDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var ease = property.FindPropertyRelative("ease");
            if (ease.intValue == 1) return EditorGUIUtility.singleLineHeight * 2;
            return EditorGUIUtility.singleLineHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var ease = property.FindPropertyRelative("ease");
            if (ease.intValue != 1)
            {
                EditorGUI.PropertyField(position, ease);
                return;
            }

            position.height *= 0.5f;
            EditorGUI.PropertyField(position, ease);
            position.y += position.height;
            position.x += 10f;
            position.width -= 10f;
            EditorGUI.PropertyField(position, property.FindPropertyRelative("curve"));
        }
    }
}