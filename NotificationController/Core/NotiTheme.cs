using UnityEngine;

namespace Omnix.Notification
{
    [CreateAssetMenu]
    public class NotiTheme : ScriptableObject
    {
        
        [SerializeField] private LoadingScreen loadingScreen;
        [SerializeField] private MessageScreen messageScreen;
        [SerializeField] private MessageScreen successScreen;
        [SerializeField] private MessageScreen errorScreen;
        [SerializeField] private ConfirmScreen confirmScreen;
        
        public LoadingScreen LoadingScreen => loadingScreen;
        public MessageScreen MessageScreen => messageScreen;
        public MessageScreen SuccessScreen => successScreen;
        public MessageScreen ErrorScreen => errorScreen;
        public ConfirmScreen ConfirmScreen => confirmScreen;
    }

}