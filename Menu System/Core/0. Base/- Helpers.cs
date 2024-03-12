using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;
namespace MenuManagement.Base
{
    /// <summary> Helper methods </summary>
    public static class Helpers
    {
        /// <summary> Destroys all the children of this gameObject </summary>
        public static void DestroyAllChildren(Transform parent)
        {
            if (parent.childCount == 0) return;

            foreach (Transform child in parent)
            {
                Object.Destroy(child.gameObject);
            }
        }

        public static void SetupHoverCallbacks(GameObject item, UnityAction<BaseEventData> onEnter, UnityAction<BaseEventData> onExit, UnityAction<BaseEventData> onClick)
        {
            EventTrigger eventTrigger = item.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerEnter = new EventTrigger.Entry();
            pointerEnter.eventID = EventTriggerType.PointerEnter;
            pointerEnter.callback.AddListener(onEnter);
            eventTrigger.triggers.Add(pointerEnter);

            EventTrigger.Entry pointerExit = new EventTrigger.Entry();
            pointerExit.eventID = EventTriggerType.PointerExit;
            pointerExit.callback.AddListener(onExit);
            eventTrigger.triggers.Add(pointerExit);
            
            EventTrigger.Entry pointerClick = new EventTrigger.Entry();
            pointerClick.eventID = EventTriggerType.PointerClick;
            pointerClick.callback.AddListener(onClick);
            eventTrigger.triggers.Add(pointerClick);
        }
    }
}