using UnityEngine;
using UnityEngine.UI;

namespace MenuManagement.Layouts
{
    [ExecuteAlways]
    public class CircularLayoutGroup : LayoutGroup
    {
        public Vector2 centerOffset = Vector2.zero;
        public bool clockwise = true;
        public float startAngle = 0f;
        public float endAngle = 360f;

        private void ArrangeChildren()
        {
            int numElements = transform.childCount;
            float angleStep = (endAngle - startAngle) / numElements;
            float currentAngle = startAngle;
            
            Rect rect = GetComponent<RectTransform>().rect;
            rect.width = rect.width * 0.5f - padding.horizontal;
            rect.height = rect.height * 0.5f - padding.vertical;
            
            Vector2 centerPos = ((Vector2)transform.position) + centerOffset;
            centerPos.x = centerPos.x + padding.left - padding.right;
            centerPos.y = centerPos.y + padding.bottom - padding.top;
            
            for (int i = 0; i < numElements; i++)
            {
                float xPos = centerPos.x + Mathf.Cos(Mathf.Deg2Rad * currentAngle) * rect.width;
                float yPos = centerPos.y + Mathf.Sin(Mathf.Deg2Rad * currentAngle) * rect.height;

                RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
                child.position = new Vector3(xPos, yPos, 0f);
                
                // if (!clockwise) child.localScale = new Vector3(-1f, 1f, 1f);
                currentAngle += clockwise ? angleStep : -angleStep;
            }
        }

        protected override void OnEnable() { base.OnEnable(); ArrangeChildren(); }
        public override void CalculateLayoutInputVertical() { ArrangeChildren(); }
        public override void CalculateLayoutInputHorizontal() { ArrangeChildren(); }
        public override void SetLayoutHorizontal() { }
        public override void SetLayoutVertical() { }
        
        #if UNITY_EDITOR
        protected override void OnValidate()
        {
            base.OnValidate();
            ArrangeChildren();
        }

        protected override void Reset()
        {
            base.Reset();
            ArrangeChildren();
        }
        #endif
    }
}