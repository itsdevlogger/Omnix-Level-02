using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Omnix.Notification
{
    public class MessageScreen : BaseScreen
    {
        [SerializeField, Tooltip("[NOT NULL] Text that represents title of this window")] private TextMeshProUGUI _title;
        [SerializeField, Tooltip("[NOT NULL] Text that represents description of this window")] private TextMeshProUGUI _description;
        [SerializeField, Tooltip("[Can Be Null] Close window button"), CanBeNull] private Button _closeButton;
        [SerializeField, Tooltip("[Can Be Null] Yes Option Button"), CanBeNull] private ButtonAndText _okButton;
        
        private void Awake()
        {
            _okButton?.AddListener(Close);
            if (_closeButton) _closeButton.onClick.AddListener(Close);
        }

        public void Init(string title, string description, IButtonSettings okSettings, UnityAction onCloseClicked, float autoHideDuration)
        {
            Activate(autoHideDuration);
            if (_title) _title.SetText(title);
            if (_description) _description.SetText(description);
            if (onCloseClicked != null && _closeButton != null) _closeButton.onClick.AddListener(onCloseClicked);
            _okButton?.Set(okSettings);
        }
    }
}