using TMPro;
using UnityEngine;

namespace Omnix.Notification
{
    /// <summary>
    /// A base class for all notification screens
    /// </summary>
    public class BaseNotificationScreen : MonoBehaviour
    {
        // The text component to display the message
        [SerializeField] protected TextMeshProUGUI titleText;
        internal bool destroyOnHide;
        public static BaseNotificationScreen Current { get; private set; }

        public void Close(float delay = 0)
        {
            if (delay <= 0) return;
            Invoke(nameof(CloseDelayed), delay);
        }

        public static void Activate(BaseNotificationScreen screen)
        {
            Current = screen;
            screen.gameObject.SetActive(true);
        }

        private void CloseDelayed()
        {
            if (Current == this)
            {
                Notification.CloseActiveScreen();
            }
        }

        internal static void HideCurrent()
        {
            if (Current == null) return;
            if (Current.destroyOnHide) Destroy(Current.gameObject);
            else Current.gameObject.SetActive(false);
            Current = null;
        }
    }
}