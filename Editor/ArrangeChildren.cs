using System.Collections.Generic;
using UnityEngine;
using Omnix.Utils.EditorUtils;
using UnityEditor;
using Random = UnityEngine.Random;

namespace Omnix.Monos
{
    [CustomPropertyDrawer(typeof(ArrangeOnAxis))]
    public class ArrangeOnAxisDrawer : PropertyDrawer
    {
        private static readonly Dictionary<int, string> val1Names = new Dictionary<int, string>()
        {
            { 0, "-" }, // ZigZag,
            { 1, "Zig" }, // ZigZag,
            { 2, "Start" }, // StartStep,
            { 3, "Start" }, // StartEnd,
            { 4, "Center" }, // CenterStep,
            { 5, "Minimum" } // Scatter
        };

        private static readonly Dictionary<int, string> val2Names = new Dictionary<int, string>()
        {
            { 0, "-" }, // ZigZag,
            { 1, "Zag" }, // ZigZag,
            { 2, "Step" }, // StartStep,
            { 3, "End" }, // StartEnd,
            { 4, "Step" }, // CenterStep,
            { 5, "Maximum" } // Scatter
        };

        SerializedProperty mode;
        SerializedProperty val1;
        SerializedProperty val2;
        SerializedProperty alternate;
        private Rect rect;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            mode ??= property.FindPropertyChild("mode");
            val1 ??= property.FindPropertyChild("val1");
            val2 ??= property.FindPropertyChild("val2");
            alternate ??= property.FindPropertyChild("alternate");

            if (!property.isExpanded) return EditorGUIUtility.singleLineHeight;
            return mode.enumValueIndex switch
            {
                0 => EditorGUIUtility.singleLineHeight,
                5 => EditorGUIUtility.singleLineHeight * 8f,
                _ => EditorGUIUtility.singleLineHeight * 5f
            };
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            rect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight - 2f);
            property.isExpanded = EditorGUI.Foldout(rect, property.isExpanded, label);

            rect.x += 50f;
            rect.width -= 50;
            EditorGUI.PropertyField(rect, mode, GUIContent.none);
            rect.x -= 50f;
            rect.width += 50;

            if (!property.isExpanded || (mode.enumValueIndex == 0)) return;

            EditorGUI.PropertyField(Rect, val1, new GUIContent(val1Names[mode.enumValueIndex]));
            EditorGUI.PropertyField(Rect, val2, new GUIContent(val2Names[mode.enumValueIndex]));
            EditorGUI.PropertyField(Rect, alternate);

            if (mode.enumValueIndex == 5)
            {
                if (GUI.Button(Rect, "Scatter"))
                {
                    Scatter((ArrangeChildren)property.serializedObject.targetObject, label.text);
                }
                rect.height *= 2f;
                EditorGUI.HelpBox(Rect, "Scatter is an editor only option.", MessageType.Error);
            }

            EditorGUI.EndProperty();
        }

        private void Scatter(ArrangeChildren target, string label)
        {
            int counter = -1;
            int alter = alternate.intValue;
            foreach (Transform child in target.transform)
            {
                counter++;
                if ((alter > 1) && (counter % alter != 0)) continue;
                if (target.ArrangeEnabledOnly && !child.gameObject.activeSelf) continue;

                Vector3 current = target.ActsOn switch
                {
                    ArrangeChildren.ArrangeMode.Position => child.localPosition,
                    ArrangeChildren.ArrangeMode.Rotation => child.localRotation.eulerAngles,
                    _ => child.localScale
                };

                switch (label)
                {
                    case "X":
                        current.x = Random.Range(val1.floatValue, val2.floatValue);
                        break;
                    case "Y":
                        current.y = Random.Range(val1.floatValue, val2.floatValue);
                        break;
                    case "Z":
                        current.z = Random.Range(val1.floatValue, val2.floatValue);
                        break;
                }

                switch (target.ActsOn)
                {
                    case ArrangeChildren.ArrangeMode.Position:
                        child.localPosition = current;
                        break;
                    case ArrangeChildren.ArrangeMode.Rotation:
                        child.localRotation = Quaternion.Euler(current);
                        break;
                    default:
                        child.localScale = current;
                        break;
                }

                EditorUtility.SetDirty(child);
            }
            EditorUtility.SetDirty(target);
        }

        Rect Rect
        {
            get
            {
                rect.y += EditorGUIUtility.singleLineHeight;
                return rect;
            }
        }
    }
}