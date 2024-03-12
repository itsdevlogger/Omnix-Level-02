using System.Collections.Generic;
using MenuManagement.Base;
using MenuManagement.Transitions;
using UnityEditor;


namespace MenuManagement.Editor
{
    [CustomEditor(typeof(TransformTransition)), CanEditMultipleObjects]
    public class TransitionTransformEditor : BaseTransitionBlendableEditor
    {
        private SerializedProperty targetsProp;
        private SerializedProperty loadStartProp;
        private SerializedProperty unloadEndProp;
        private BasePropertyGroupDrawer drawer;

        protected override IEnumerable<BasePropertyGroupDrawer> RegisterTransitionGroups()
        {
            SerializedProperty extremeProp = GrabProperty("extreme");
            targetsProp = GrabProperty("targets");
            loadStartProp = extremeProp.FindPropertyRelative("loadStart");
            unloadEndProp = extremeProp.FindPropertyRelative("unloadEnd");

            drawer = new BasePropertyGroupDrawer("End Points", "Should not be null")
            {
                targetsProp,
                loadStartProp,
                extremeProp.FindPropertyRelative("loadEnd"),
                extremeProp.FindPropertyRelative("unloadStart"),
                unloadEndProp,
            };
            CheckConflicts();
            yield return drawer;
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
            TransformTransitionTargets flags = (TransformTransitionTargets)targetsProp.intValue;
            bool hasLocalPos = flags.HasFlag(TransformTransitionTargets.LocalPosition);
            bool hasGlobalPos = flags.HasFlag(TransformTransitionTargets.GlobalPosition);
            bool hasUiBased =
                flags.HasFlag(TransformTransitionTargets.RectAnchorPosition) ||
                flags.HasFlag(TransformTransitionTargets.RectPivot) ||
                flags.HasFlag(TransformTransitionTargets.RectAnchorMin) ||
                flags.HasFlag(TransformTransitionTargets.RectAnchorMax);

            if (hasLocalPos && hasGlobalPos && hasUiBased)
            {
                drawer.SetErrorMessage($"Multiple position setters are selected. Select exactly one of the following:\n" +
                                       "   - LocalPosition\n" +
                                       "   - GlobalPosition\n" +
                                       "   - Any number of Rect Properties");
            }
            else if (hasLocalPos && hasGlobalPos)
            {
                drawer.SetErrorMessage($"Multiple position setters are selected. Select exactly one of the following:\n" +
                                       "   - LocalPosition\n" +
                                       "   - GlobalPosition");
            }
            else if (hasLocalPos && hasUiBased)
            {
                drawer.SetErrorMessage($"Multiple position setters are selected. Select exactly one of the following:\n" +
                                       "   - LocalPosition\n" +
                                       "   - Any number of Rect Properties");
            }
            else if (hasGlobalPos && hasUiBased)
            {
                drawer.SetErrorMessage($"Multiple position setters are selected. Select exactly one of the following:\n" +
                                       "   - GlobalPosition\n" +
                                       "   - Any number of Rect Properties");
            }
            else if (flags.HasFlag(TransformTransitionTargets.LocalRotation) && flags.HasFlag(TransformTransitionTargets.GlobalPosition))
            {
                drawer.SetErrorMessage($"Multiple rotation setters are selected. Select exactly one of the following:\n" +
                                       "   - LocalRotation\n" +
                                       "   - GlobalPosition");
            }
            else if (loadStartProp.objectReferenceValue == null)
            {
                drawer.SetErrorMessage($"loadStart property cannot be null");
            }
            else if (unloadEndProp.objectReferenceValue == null)
            {
                drawer.SetErrorMessage($"unloadEnd property cannot be null");
            }
            else
            {
                drawer.SetErrorMessage(null);
            }
        }
    }
}