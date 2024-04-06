using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Omnix.Notification
{
    public class BaseScreen : MonoBehaviour
    {
        public static BaseScreen CurrentScreen { get; private set; }
        public static int CurrentScreenId { get; private set; } 
        public int ScreenId { get; private set; }
        [NonSerialized] public bool destroyOnClose;

        
        protected void Activate(float autoHideDuration)
        {
            CurrentScreen = this;
            CurrentScreenId = Random.Range(int.MinValue, int.MaxValue);
            Debug.Log($"CurrentScreenId: {CurrentScreenId}");
            ScreenId = CurrentScreenId;
            gameObject.SetActive(true);
            if (autoHideDuration > Time.deltaTime) NotiCon.Instance.AutohideActiveScreen(autoHideDuration);
        }

        internal void Close()
        {
            if (destroyOnClose) Destroy(gameObject);
            else gameObject.SetActive(false);
            CurrentScreen = null;
            NotiCon.Instance.ScreenClosed();
        }
    }
}