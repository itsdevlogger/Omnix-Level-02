using UnityEngine;
using UnityEngine.EventSystems;

namespace Omnix.Monos
{
    public class UiDraggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        [SerializeField] private GameObject dropAllowedIndicator;
        [SerializeField] private GameObject dropDeniedIndicator;


        private RectTransform rectTransform;
        private Canvas canvas;
        private CanvasGroup canvasGroup;
        private Vector3 startPosition;
        private Transform startParent;
        private bool hasCanvasGroup;

        // Event triggered when the user starts dragging a UiDraggable object
        // public static event System.Action OnDragBegin;

        // Event triggered when the user ends dragging a UiDraggable object
        // public static event System.Action OnDragEnd;
        private UiDroppable currentDropTarget;

        private void Awake()
        {
            // Get the RectTransform, Canvas, and CanvasGroup components
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            canvasGroup = GetComponent<CanvasGroup>();

            hasCanvasGroup = (canvasGroup != null);
            dropAllowedIndicator.SetActive(false);
            dropDeniedIndicator.SetActive(false);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            // Save the starting position of the UiDraggable object
            startPosition = rectTransform.localPosition;
            startParent = rectTransform.parent;

            // Reduce the opacity and disable raycasting of the UiDraggable object
            if (hasCanvasGroup)
            {
                canvasGroup.alpha = 0.6f;
                canvasGroup.blocksRaycasts = false;
            }

            transform.SetParent(canvas.transform);
            dropAllowedIndicator.SetActive(false);
            dropDeniedIndicator.SetActive(true);
            currentDropTarget = null;
            // Trigger the OnDragBegin event
            // if (OnDragBegin != null)
            // OnDragBegin.Invoke();
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Move the UiDraggable object according to the mouse movement
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;

            if (eventData.pointerCurrentRaycast.gameObject != null && eventData.pointerCurrentRaycast.gameObject.TryGetComponent(out UiDroppable droppable))
            {
                if (droppable == currentDropTarget) return;

                if (currentDropTarget) currentDropTarget.HideActivePanel();
                currentDropTarget = droppable;
                droppable.ShowActivePanel();
                dropAllowedIndicator.SetActive(true);
                dropDeniedIndicator.SetActive(false);
                return;
            }
            currentDropTarget = null;
            dropAllowedIndicator.SetActive(false);
            dropDeniedIndicator.SetActive(true);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            // Restore the opacity and raycasting of the UiDraggable object
            if (hasCanvasGroup)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.blocksRaycasts = true;
            }


            // Check if the UiDraggable object is dropped on a UiDroppable object
            if (currentDropTarget != null)
            {
                currentDropTarget.OnDrop(gameObject);
            }
            else
            {
                rectTransform.localPosition = startPosition;
                rectTransform.SetParent(startParent);
            }

            dropAllowedIndicator.SetActive(false);
            dropDeniedIndicator.SetActive(false);
            currentDropTarget = null;
        }
    }
}