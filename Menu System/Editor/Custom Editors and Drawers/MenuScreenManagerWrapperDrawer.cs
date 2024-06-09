using MenuManagement.Perception;
using UnityEditor;
using UnityEngine;

namespace MenuManagement.Editor
{
    [CustomPropertyDrawer(typeof(MenuScreenManager.Wrapper))]
    public class MenuScreenManagerWrapperDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            SerializedProperty menuProp = property.FindPropertyRelative("menu");
            SerializedProperty toggleProp = property.FindPropertyRelative("toggle");

            position.width = position.width * 0.5f - 5f;
            EditorGUI.PropertyField(position, toggleProp, GUIContent.none);
            position.x += position.width + 5f;
            EditorGUI.PropertyField(position, menuProp, GUIContent.none);
        }
    }
}