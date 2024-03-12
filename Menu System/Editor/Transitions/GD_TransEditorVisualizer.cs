using System;
using System.Collections;
using MenuManagement.Base;
using UnityEditor;
using UnityEngine;

namespace MenuManagement.Editor
{
    public class GD_TransEditorVisualizer : BasePropertyGroupDrawer
    {
        private static BaseMenu LoadMenu;
        private static BaseMenu UnloadMenu;
        private BaseTransitionBlendable transition;
        private BaseTransitionBlendableEditor parent;
        public bool IsGuiEnabled { get; private set; }
        
        public GD_TransEditorVisualizer(BaseTransitionBlendable transition, BaseTransitionBlendableEditor parent) : base(_.TransitionEditorVisualizer)
        {
            this.IsGuiEnabled = true;
            this.transition = transition;
            this.parent = parent;
            Add(null);
        }

        protected override void DrawGroup()
        {
            LoadMenu = EditorGUILayout.ObjectField("Loading Menu", LoadMenu, typeof(BaseMenu), true) as BaseMenu;
            UnloadMenu = EditorGUILayout.ObjectField("Unloading Menu", UnloadMenu, typeof(BaseMenu), true) as BaseMenu;
            bool enabled = GUI.enabled;
            GUI.enabled = (LoadMenu != null) || (UnloadMenu != null);
            if (GUILayout.Button("Play Animation"))
            {
                EditorCoroutine.StartCoroutine(PlayAnimationCoroutine());
            }
            GUI.enabled = enabled;
            if (GUILayout.Button("Play Load On This Object"))
            {
                LoadMenu = transition.GetComponent<BaseMenu>();
                if (LoadMenu != null)
                {
                    if (UnloadMenu == LoadMenu) UnloadMenu = null;
                    EditorCoroutine.StartCoroutine(PlayAnimationCoroutine());
                }
                else EditorUtility.DisplayDialog("Failed", "GameObject has no menu attached.", "Okay");
            }
            
            if (GUILayout.Button("Play Unload On This Object"))
            {
                UnloadMenu = transition.GetComponent<BaseMenu>();
                if (UnloadMenu != null)
                {
                    if (UnloadMenu == LoadMenu) LoadMenu = null;
                    EditorCoroutine.StartCoroutine(PlayAnimationCoroutine());
                }
                else EditorUtility.DisplayDialog("Failed", "GameObject has no menu attached.", "Okay");
            }
        }

        private IEnumerator PlayAnimationCoroutine()
        {
            IsGuiEnabled = false;
            parent.Repaint();
            IEnumerator animator;

            try
            {
                animator = transition.Animate(UnloadMenu, LoadMenu);
            }
            catch (Exception)
            {
                EditorUtility.DisplayDialog("Error", "Something went wrong. Have you assigned all the essential variables?", "Okay");
                IsGuiEnabled = true;
                parent.Repaint();
                yield break;
            }
            
            
            int count = 0;
            while (true)
            {
                try
                {
                    if (animator.MoveNext() == false)
                    {
                        break;
                    }
                }
                catch (Exception)
                {
                    EditorUtility.DisplayDialog("Error", "Something went wrong. Have you assigned all the essential variables?", "Okay");
                    break;   
                }

                count++;
                if (count > 1000) break;
                yield return null;
            }

            transition.Cleanup(UnloadMenu, LoadMenu);
            IsGuiEnabled = true;
            parent.Repaint();
        }
    }
}