using MenuManagement.Base;
using UnityEditor;
using UnityEngine;

namespace MenuManagement.Editor
{
    public class GD_TransCommonsGroup : BasePropertyGroupDrawer
    {
        private static readonly Color RedText = new Color(1f, 0.731f, 0f, 1f);
        private static readonly Color RedBar = new Color(1f, 0.731f, 0f, 0.25f);
        private static readonly Color GreenText = new Color(0.269f, 1f, 0f, 1f);
        private static readonly Color GreenBar = new Color(0.269f, 1f, 0f, 0.25f);

        private SerializedProperty blendProp;

        public GD_TransCommonsGroup(SerializedProperty blendProp) : base(_.BaseTransitionProperties)
        {
            this.blendProp = blendProp;
        }

        protected override void DrawGroup()
        {
            base.DrawGroup();
            DrawBlend(blendProp.floatValue);
        }

        private static void DrawBlend(float blend)
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 36f);
            rect.width -= 36f;
            rect.x += 18f;

            // Draw the labels
            rect.height = 18f;
            GUI.color = RedText;
            GUI.Label(rect, "Unloading Menu Animation");
            rect.y += 18f;
            GUI.color = GreenText;
            GUI.Label(rect, "Loading Menu Animation");
            GUI.color = Color.white;
            rect.y -= 18f;

            if (blend >= 0)
            {
                float widthFract = 1 - blend * 0.5f;
                float xOff = rect.width * blend * 0.5f;

                rect.width *= widthFract;
                EditorGUI.DrawRect(rect, RedBar);
                rect.y += rect.height;
                rect.x += xOff;
                EditorGUI.DrawRect(rect, GreenBar);
            }
            else
            {
                blend *= -1;
                float widthFract = 1 - blend * 0.5f;
                float xOr = rect.x;

                rect.x += rect.width * blend * 0.5f;
                rect.width *= widthFract;
                EditorGUI.DrawRect(rect, RedBar);
                rect.x = xOr;
                rect.y += rect.height;
                EditorGUI.DrawRect(rect, GreenBar);
                rect.y -= rect.height;
            }
        }
    }
}