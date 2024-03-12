using System;
using System.Collections.Generic;
using System.Reflection;
using MenuManagement.Base;
using MenuManagement.Behaviours;
using UnityEditor;
using UnityEngine;

namespace MenuManagement.Editor
{
    public class GD_DynamicMenuSettingsDrawer : BasePropertyGroupDrawer
    {
        private static GUIContent filterContent = new GUIContent("Filter Method", "Select which filter function to use with this menu");

        private SerializedProperty loadInAwake;
        private SerializedProperty thisObjectActiveWithMenu;
        private SerializedProperty allowDeselect;
        private SerializedProperty behaviour;
        private SerializedProperty filterIndex;
        private SerializedProperty itemPrefab;
        private SerializedProperty itemsParent;
        private SerializedProperty confirmButton;
        private SerializedProperty selected;
        private GUIContent[] filterOptions;


        public GD_DynamicMenuSettingsDrawer(BaseDynamicMenuEditor editor) : base(_.DynamicMenuProperties)
        {
            thisObjectActiveWithMenu = editor.GrabProperty("thisObjectActiveWithMenu");
            loadInAwake = editor.GrabProperty("loadInAwake");
            allowDeselect = editor.GrabProperty("allowDeselect");
            behaviour = editor.GrabProperty("behaviour");
            filterIndex = editor.GrabProperty("filterIndex");
            itemPrefab = editor.GrabProperty("itemPrefab");
            itemsParent = editor.GrabProperty("itemsParent");
            confirmButton = editor.GrabProperty("confirmButton");
            selected = editor.GrabProperty("<Selected>k__BackingField");


            List<GUIContent> options = new List<GUIContent>();
            options.Add(new GUIContent("No Filter"));
            Type type = editor.target.GetType();
            for (int i = 1; i < 11; i++)
            {
                string methodName = $"Filter{i}";

                MethodInfo methodInfo = type.GetMethod(methodName, BindingFlags.Default | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);
                if (methodInfo == null) continue;

                FilterFunctionAttribute att = null;
                foreach (Attribute attribute in methodInfo.GetCustomAttributes())
                {
                    if (attribute.GetType() == typeof(FilterFunctionAttribute))
                    {
                        att = (FilterFunctionAttribute)attribute;
                        break;
                    }
                }

                if (att == null) options.Add(new GUIContent(methodName));
                else options.Add(new GUIContent(att.name, att.description));
            }

            filterOptions = options.ToArray();
            


            CheckStatus();
        }

        private string GetInfo(int beh)
        {
            return beh switch
            {
                0 => "Child items can't be selected or confirmed.",
                1 => "Click on child to select, and click on ConfirmButton to confirm.",
                2 => "Hover on a child to select it, and Click on a child to confirm.",
                _ => throw new ArgumentOutOfRangeException(nameof(beh), beh, null)
            };
        }

        private void CheckStatus()
        {
            if (itemPrefab.objectReferenceValue == null) SetErrorMessage("Field itemPrefab is not assigned");
            else if (itemsParent.objectReferenceValue == null) SetErrorMessage("Field itemsParent is not assigned");
            else SetErrorMessage(null);
        }

        protected override void DrawGroup()
        {
            bool guiEnambed = GUI.enabled;
            GUI.enabled = !Application.isPlaying;

            // Toggles
            EditorGUILayout.PropertyField(loadInAwake);
            EditorGUILayout.PropertyField(thisObjectActiveWithMenu);

            // Filter
            filterIndex.intValue = EditorGUILayout.Popup(filterContent, filterIndex.intValue, filterOptions);
            var filter = filterOptions[filterIndex.intValue];
            if (string.IsNullOrEmpty(filter.tooltip) == false)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField(filter.tooltip, EditorStyles.miniLabel);
                EditorGUI.indentLevel--;
            }

            // Behaviour
            EditorGUILayout.PropertyField(behaviour);
            EditorGUI.indentLevel++;
            int beh = behaviour.intValue;
            EditorGUILayout.LabelField(GetInfo(beh), EditorStyles.miniLabel);
            if (beh != 0) EditorGUILayout.PropertyField(allowDeselect);
            if (beh == 1) EditorGUILayout.PropertyField(confirmButton);
            if (beh != 0) EditorGUILayout.PropertyField(selected);
            EditorGUI.indentLevel--;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(itemPrefab);
            EditorGUILayout.PropertyField(itemsParent);
            if (EditorGUI.EndChangeCheck()) CheckStatus();

            
            GUI.enabled = guiEnambed;

            base.DrawGroup();
        }
    }
}