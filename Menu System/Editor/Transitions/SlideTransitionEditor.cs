using System.Collections.Generic;
using MenuManagement.Base;
using MenuManagement.Transitions;
using UnityEditor;

namespace MenuManagement.Editor
{
    [CustomEditor(typeof(SlideTransition))]
    public class SlideTransitionEditor : BaseTransitionBlendableEditor
    {
        private SerializedProperty loadStart;
        private SerializedProperty unloadEnd;
        private BasePropertyGroupDrawer group;

        protected override IEnumerable<BasePropertyGroupDrawer> RegisterTransitionGroups()
        {
            loadStart = GrabProperty("loadStartTrans");
            unloadEnd = GrabProperty("unloadEndTrans");

            group = new BasePropertyGroupDrawer(_.EndPoints)
            {
                loadStart,
                unloadEnd
            };
            CheckConflicts();
            yield return group;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            base.OnInspectorGUI();
            if (EditorGUI.EndChangeCheck())
            {
                CheckConflicts();
            }
        }

        private void CheckConflicts()
        {
            if (loadStart.objectReferenceValue == null)
            {
                group.SetErrorMessage("loadStart property is not set");
            }
            else if (unloadEnd.objectReferenceValue == null)
            {
                group.SetErrorMessage("unloadEnd property is not set");
            }
            else
            {
                group.SetErrorMessage(null);
            }
        }
    }
}