using System;
/*
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
*/

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

        public string title;
        public string details;
        public float autohideDuration;
        public Type type;
        public ButtonConfigSerializable okayButton = new ButtonConfigSerializable("Okay");
        public ButtonConfigSerializable noButton = new ButtonConfigSerializable("No");
        public ButtonConfigSerializable cancelButton = new ButtonConfigSerializable("Cancel");
    }
    
    /*
    #if UNITY_EDITOR
    // [CustomPropertyDrawer(typeof(NotificationInfo))]
    public class NotificationInfoDrawer : PropertyDrawer
    {
        private GUIContent yesLabel;

        private SerializedProperty title;
        private SerializedProperty type;
        private SerializedProperty details;
        private SerializedProperty okayButton;
        private SerializedProperty noButton;
        private SerializedProperty cancelButton;
        private SerializedProperty autohideDuration;
        private bool drawConfirmScreen;
        private float okayHeight;
        private float noHeight;
        private float cancelHeight;
        private Rect _pos;
        private float _lastHeight;

        private Rect Position
        {
            get
            {
                _pos.y += _lastHeight;
                _lastHeight = _pos.height;
                return _pos;
            }
        }
        
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            type = property.FindPropertyRelative("type");
            title = property.FindPropertyRelative("title");
            details = property.FindPropertyRelative("details");
            okayButton = property.FindPropertyRelative("okayButton");
            noButton = property.FindPropertyRelative("noButton");
            cancelButton = property.FindPropertyRelative("cancelButton");
            autohideDuration = property.FindPropertyRelative("autohideDuration");
            drawConfirmScreen = (type.enumValueIndex == 3);
            
            okayHeight = EditorGUI.GetPropertyHeight(okayButton);
            float h = EditorGUIUtility.singleLineHeight * 4f + okayHeight;
            if (drawConfirmScreen == false) return h;

            noHeight = EditorGUI.GetPropertyHeight(noButton);
            cancelHeight = EditorGUI.GetPropertyHeight(cancelButton);
            return h + noHeight + cancelHeight;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (yesLabel == null)
            {
                yesLabel = new GUIContent("Yes");
            }

            _lastHeight = 0;
            _pos = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.PropertyField(Position, title);
            EditorGUI.PropertyField(Position, type);
            EditorGUI.PropertyField(Position, details);
            EditorGUI.PropertyField(Position, autohideDuration);

            _pos.height = okayHeight;
            if (drawConfirmScreen)
            {
                EditorGUI.PropertyField(Position, okayButton, yesLabel, true);
                _pos.height = noHeight;
                EditorGUI.PropertyField(Position, noButton, true);
                _pos.height = cancelHeight;
                EditorGUI.PropertyField(Position, cancelButton, true);
            }
            else
            {
                EditorGUI.PropertyField(Position, okayButton, true);
            }
        }
    }
    #endif*/
}