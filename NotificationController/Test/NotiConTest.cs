using Omnix.Notification;
using UnityEngine;

public class NotiConTest : MonoBehaviour
{
    [SerializeField] private NotificationInfo[] infos;
    
    
    void Start()
    {
        foreach (NotificationInfo info in infos)
        {
            NotiCon.Show(info);
        }
        
        NotiCon.Info("[01] Info", "This will hide in 2 seconds", autoHideDuration: 2f);
        NotiCon.Info("[02] Info", "Message will continue till you end this. No callback");
        NotiCon.Info("[03] Info", "Message will continue till you end this. With callback", onOkayClicked: () => Debug.Log("Clicked [03]"));
        NotiCon.Info("[04] Info", "Message will continue till you end this. With callback. Okay is yes.", onOkayClicked: () => Debug.Log("Clicked [04]"), okayButtonText: "Yes");
        NotiCon.Info("[05] Info", "Message will auto hide in 3 seconds. With callback. Okay is yes.", onOkayClicked: () => Debug.Log("Clicked [05]"), okayButtonText: "Yes", autoHideDuration: 3f);

        NotiCon.Success("[06] Success", "Success will continue till you end this. No callback");
        NotiCon.Success("[07] Success", "Success will continue till you end this. With callback",  onOkayClicked: () => Debug.Log("Clicked [07]"));
        NotiCon.Success("[08] Success", "Success will continue till you end this. With callback. Okay is yes.", onOkayClicked: () => Debug.Log("Clicked [08]"),  okayButtonText:"Yes");
        NotiCon.Success("[09] Success", "Success will auto hide in 3 seconds. With callback. Okay is yes.", onOkayClicked: () => Debug.Log("Clicked [09]"),  okayButtonText:"Yes", autoHideDuration: 3f);

        NotiCon.Error("[10] Error", "Error will continue till you end this. No callback");
        NotiCon.Error("[11] Error", "Error will continue till you end this. With callback", onOkayClicked: () => Debug.Log("Clicked [11]"));
        NotiCon.Error("[12] Error", "Error will continue till you end this. With callback. Okay is yes.", onOkayClicked: () => Debug.Log("Clicked [12]"),  okayButtonText: "Yes");
        NotiCon.Error("[13] Error", "Error will auto hide in 3 seconds. With callback. Okay is yes.", onOkayClicked: () => Debug.Log("Clicked [13]"), okayButtonText: "Yes", autoHideDuration:3f);
        
        // Test Confirm
        // Test Chain
        // Test MessageInfos
        // Test all that with multiple themes
    }

    [ContextMenu("ShowLoading")]
    private void ShowLoading()
    {
        NotiCon.ShowLoading();
    }
    
    [ContextMenu("HideLoading")]
    private void HideLoading()
    {
        NotiCon.HideLoading();
    }
}
