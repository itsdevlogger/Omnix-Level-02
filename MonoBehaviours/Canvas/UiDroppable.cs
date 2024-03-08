using UnityEngine;

namespace Omnix.Monos
{
    public class UiDroppable : MonoBehaviour
    {
        [SerializeField, Tooltip("GameObject to activate to indicate that the user can drop item here")] 
        private GameObject dragIndicator;

        private void Awake()
        {
            // Make the inactive panel active by default
            HideActivePanel();
        }


        /*
        private void OnEnable()
        {
            UiDraggable.OnDragBegin += ShowActivePanel;
            UiDraggable.OnDragEnd += HideActivePanel;
        }

        private void OnDisable()
        {
            UiDraggable.OnDragBegin -= ShowActivePanel;
            UiDraggable.OnDragEnd -= HideActivePanel;
        }*/

        public void ShowActivePanel()
        {
            // Show active panel and hide inactive panel
            dragIndicator.SetActive(true);
        }

        public void HideActivePanel()
        {
            // Show inactive panel and hide active panel
            dragIndicator.SetActive(false);
        }

        public void OnDrop(GameObject target)
        {
            // if (eventData.pointerDrag == null) return;
            RectTransform draggableRectTransform = target.GetComponent<RectTransform>();
            draggableRectTransform.SetParent(transform);
            // draggableRectTransform.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;
            HideActivePanel();
        }
    }
}