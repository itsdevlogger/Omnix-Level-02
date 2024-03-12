using System.Collections.Generic;
using MenuManagement.Base;
using UnityEditor;
using UnityEngine;

namespace MenuManagement.Editor
{
    [CustomEditor(typeof(BaseTransitionBlendable), editorForChildClasses: true)]
    [CanEditMultipleObjects]
    public class BaseTransitionBlendableEditor : BaseEditorWithGroups
    {
        private GD_TransEditorVisualizer transEditorVisualizer;
        
        protected sealed override IEnumerable<BasePropertyGroupDrawer> RegisterGroups()
        {
            SerializedProperty inOutBlendProp = GrabProperty("inOutBlend");
            yield return new GD_TransCommonsGroup(inOutBlendProp)
            {
                GrabProperty("transitionOrders"),
                GrabProperty("transitionDuration"),
                GrabProperty("ease"),
                inOutBlendProp
            };

            foreach (BasePropertyGroupDrawer drawer in RegisterTransitionGroups())
            {
                yield return drawer;
            }
            
            foreach (BasePropertyGroupDrawer group in base.RegisterGroups())
            {
                yield return group;
            }
                        
            transEditorVisualizer = new GD_TransEditorVisualizer((BaseTransitionBlendable)target, this);
            yield return transEditorVisualizer;
        }

        protected virtual IEnumerable<BasePropertyGroupDrawer> RegisterTransitionGroups()
        {
            yield break;
        }

        public override void OnInspectorGUI()
        {
            bool enabled = GUI.enabled;
            GUI.enabled = transEditorVisualizer.IsGuiEnabled;
            base.OnInspectorGUI();
            GUI.enabled = enabled;
        }
    }
}