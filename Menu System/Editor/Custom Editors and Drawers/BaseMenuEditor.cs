using MenuManagement.Base;
using UnityEditor;
using UnityEngine;

namespace MenuManagement.Editor
{
    [CustomEditor(typeof(BaseMenu), true)]
    [CanEditMultipleObjects]
    public class BaseMenuEditor : BaseEditorWithGroups
    {
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying == false && ((MonoBehaviour)target).gameObject.activeInHierarchy == false)
            {
                EditorGUILayout.HelpBox("Please make sure the menu gameObject is active in hierarchy, even if menu is unloaded by default.", MessageType.Error);
            }

            base.OnInspectorGUI();
        }
    }
}