using System;
using UnityEngine;
using Omnix.Extensions;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Omnix.Monos
{
    public class CanvasToSphereMapper : MonoBehaviour
    {
        private enum Quadrant
        {
            positive,
            negative
        }
        [SerializeField] private Transform target;

        [Header("Canvas Bonds")]
        [SerializeField] private Vector2 canvasMin;
        [SerializeField] private Vector2 canvasMax;

        [Header("World Space")]
        [SerializeField] private Quadrant zQuadrant;
        [SerializeField] private Transform circleCenter;
        [SerializeField] private float circleRadius;
        [SerializeField] private Vector2 minAngle;
        [SerializeField] private Vector2 maxAngle;

        [Header("Controls")]
        [SerializeField] private bool lookAtCenter;
        [SerializeField] private float reactSpeed;
        [SerializeField, Range(0.01f, 0.51f)] private float visualsResolution = 0.1f;

        private Vector2 _current = new Vector2(0.5f, 0.5f);
        private Vector2 _target = new Vector2(0.5f, 0.5f);

        public void SetX(Single x) => _target.x = x;
        public void SetY(Single y) => _target.y = y;

        public void SetCircleRadius(Single r)
        {
            circleRadius = r;
            UpdatePosition();
        }
        
        private static float Square(float x) => x * x;

        private void LateUpdate()
        {
            if (Vector2.Distance(_current, _target) > 0.1f)
            {
                _current = Vector2.Lerp(_current, _target, Time.deltaTime * reactSpeed);
                UpdatePosition();
            }
        }

        private Vector3 CanvasToWorldPoint(float currentX, float currentY)
        {
            Vector3 center = circleCenter.position;

            float xAngle = currentX.ConvertRange(canvasMin.x, canvasMax.x, minAngle.x, maxAngle.x) * Mathf.Deg2Rad;
            float xDif = circleRadius * Mathf.Cos(xAngle);

            float yAngle = currentY.ConvertRange(canvasMin.y, canvasMax.y, minAngle.y, maxAngle.y) * Mathf.Deg2Rad;
            float yDif = circleRadius * Mathf.Cos(yAngle);

            float zDif = Square(circleRadius) - Square(xDif) - Square(yDif);
            float zWorld;
            if (zDif < 0) zDif = 0;
            if (zQuadrant == Quadrant.negative) zWorld = center.z - Mathf.Sqrt(zDif);
            else zWorld = center.z + Mathf.Sqrt(zDif);
            return new Vector3(center.x - xDif, center.y - yDif, zWorld);
        }

        private void UpdatePosition()
        {
            target.position = CanvasToWorldPoint(_current.x, _current.y);
            if (lookAtCenter) target.LookAt(circleCenter.position);
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            bool isNotSelected = Selection.activeGameObject != gameObject && Selection.activeTransform != target && Selection.activeTransform != circleCenter;
            if (isNotSelected || visualsResolution < 0.01f || visualsResolution > 0.5f) return;

            Vector3 lastPoint;
            Gizmos.color = Color.blue;
            for (float i = 0f; i <= 1f; i += visualsResolution)
            {
                lastPoint = CanvasToWorldPoint(0f, i);
                for (float j = visualsResolution; j <= 1f; j += visualsResolution)
                {
                    Vector3 currentPoint = CanvasToWorldPoint(j, i);
                    Gizmos.DrawLine(lastPoint, currentPoint);
                    lastPoint = currentPoint;
                }
            }

            Gizmos.color = Color.green;
            for (float i = 0f; i <= 1f; i += visualsResolution)
            {
                lastPoint = CanvasToWorldPoint(i, 0f);
                for (float j = visualsResolution; j <= 1f; j += visualsResolution)
                {
                    Vector3 currentPoint = CanvasToWorldPoint(i, j);
                    Gizmos.DrawLine(lastPoint, currentPoint);
                    lastPoint = currentPoint;
                }
            }

            Gizmos.color = Color.white;
        }
        #endif
    }
}