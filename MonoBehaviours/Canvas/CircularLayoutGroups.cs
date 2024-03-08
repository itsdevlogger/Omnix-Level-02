using UnityEngine;

namespace Omnix.Monos
{
    [ExecuteAlways]
    public class CircularLayoutGroups : MonoBehaviour
    {
        public Vector2 centerOffset = Vector2.zero;
        public bool clockwise = true;
        public float startAngle = 0f;
        public float endAngle = 360f;

        private void Update()
        {
            ArrangeUIElements();
        }

        private void ArrangeUIElements()
        {
            int numElements = transform.childCount;
            float angleStep = (endAngle - startAngle) / numElements;
            float currentAngle = startAngle;
            Rect rect = GetComponent<RectTransform>().rect;
            Vector2 offset = rect.center + centerOffset;
            for (int i = 0; i < numElements; i++)
            {
                float xPos = offset.x + Mathf.Cos(Mathf.Deg2Rad * currentAngle) * rect.width * 0.5f;
                float yPos = offset.y + Mathf.Sin(Mathf.Deg2Rad * currentAngle) * rect.height * 0.5f;

                RectTransform child = transform.GetChild(i).GetComponent<RectTransform>();
                child.anchoredPosition = new Vector2(xPos, yPos);
                ;

                if (!clockwise) child.localScale = new Vector3(-1f, 1f, 1f);
                currentAngle += clockwise ? angleStep : -angleStep;
            }
        }
    }
}