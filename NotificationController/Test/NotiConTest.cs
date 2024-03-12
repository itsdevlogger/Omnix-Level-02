using Omnix.Notification;
using UnityEngine;

public class NotiConTest : MonoBehaviour
{
    [SerializeField] private NotificationInfo[] infos;
    
    
    void Start()
    {
        foreach (NotificationInfo info in infos)
        {
            Notification.Show(info);
        }
        
        Notification.Info("[01] Info", "This will hide in 2 seconds", autohideDuration: 2f);
        Notification.Info("[02] Info", "Message will continue till you end this. No callback");
        Notification.Info("[03] Info", "Message will continue till you end this. With callback", () => Debug.Log("Clicked [03]"));
        Notification.Info("[04] Info", "Message will continue till you end this. With callback. Okay is yes.", () => Debug.Log("Clicked [04]"), "Yes");
        Notification.Info("[05] Info", "Message will auto hide in 3 seconds. With callback. Okay is yes.", () => Debug.Log("Clicked [05]"), "Yes", 3f);

        Notification.Success("[06] Success", "Success will continue till you end this. No callback");
        Notification.Success("[07] Success", "Success will continue till you end this. With callback", () => Debug.Log("Clicked [07]"));
        Notification.Success("[08] Success", "Success will continue till you end this. With callback. Okay is yes.", () => Debug.Log("Clicked [08]"), "Yes");
        Notification.Success("[09] Success", "Success will auto hide in 3 seconds. With callback. Okay is yes.", () => Debug.Log("Clicked [09]"), "Yes", 3f);

        Notification.Error("[10] Error", "Error will continue till you end this. No callback");
        Notification.Error("[11] Error", "Error will continue till you end this. With callback", () => Debug.Log("Clicked [11]"));
        Notification.Error("[12] Error", "Error will continue till you end this. With callback. Okay is yes.", () => Debug.Log("Clicked [12]"), "Yes");
        Notification.Error("[13] Error", "Error will auto hide in 3 seconds. With callback. Okay is yes.", () => Debug.Log("Clicked [13]"), "Yes", 3f);
        
        // Test Confirm
        // Test Chain
        // Test MessageInfos
        // Test all that with multiple themes
    }

    [ContextMenu("ShowLoading")]
    private void ShowLoading()
    {
        Notification.ShowLoading();
    }
    
    [ContextMenu("HideLoading")]
    private void HideLoading()
    {
        Notification.HideLoading();
    }
}
