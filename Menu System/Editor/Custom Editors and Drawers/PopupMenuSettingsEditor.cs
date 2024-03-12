using System;
using MenuManagement.Perception;
using UnityEditor;
using UnityEngine;

namespace MenuManagement.Editor
{
    [CustomEditor(typeof(PopupMenuSettings))]
    public class PopupMenuSettingsEditor : UnityEditor.Editor
    {
        private static readonly GUIContent TriggerContent = new GUIContent("Trigger");
        private SerializedProperty triggerType;
        private SerializedProperty activationButton;
        private SerializedProperty activationKeycode;

        private void OnEnable()
        {
            triggerType = serializedObject.FindProperty("triggerType");
            activationButton = serializedObject.FindProperty("activationButton");
            activationKeycode = serializedObject.FindProperty("activationKeycode");
            SetTriggerTooltip();
        }

        private void SetTriggerTooltip()
        {
            TriggerContent.tooltip = triggerType.intValue switch
            {
                0 => "Button (set in InputManager) which will trigger load/unload of the Menu.",
                1 => "Keycode which will trigger load/unload of the Menu.",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck()) SetTriggerTooltip();

            EditorGUI.BeginChangeCheck();
            EditorGUI.indentLevel++;
            if (triggerType.intValue == 0) EditorGUILayout.PropertyField(activationButton, TriggerContent);
            else EditorGUILayout.PropertyField(activationKeycode, TriggerContent);
            EditorGUI.indentLevel--;
            if (EditorGUI.EndChangeCheck()) serializedObject.ApplyModifiedProperties();
        }
    }
}