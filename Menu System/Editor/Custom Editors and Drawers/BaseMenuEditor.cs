using MenuManagement.Base;
using UnityEditor;
using UnityEngine;

namespace MenuManagement.Editor
{
    [CustomEditor(typeof(BaseMenu), true)]
    [CanEditMultipleObjects]
    public class BaseMenuEditor : BaseEditorWithGroups
    {
    }
}