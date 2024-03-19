using UnityEngine;

namespace Omnix.Notification
{
    [CreateAssetMenu]
    public class NotiTheme : ScriptableObject
    {
        public MessageScreen messageScreen;
        public MessageScreen successScreen;
        public MessageScreen errorScreen;
        public ConfirmScreen confirmScreen;
        public LoadingScreen loadingScreen;
    }
}