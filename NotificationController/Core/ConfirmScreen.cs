using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Omnix.Notification
{
    public class ConfirmScreen : BaseScreen
    {
        [SerializeField, Tooltip("[NOT NULL] Text that represents title of this window")]
        private TextMeshProUGUI _title;

        [SerializeField, Tooltip("[NOT NULL] Text that represents description of this window")]
        private TextMeshProUGUI _description;
        
        [SerializeField, Tooltip("[Can Be Null] Close window button (Function is same as Cancel Button)"), CanBeNull]
        private Button _closeButton;
        
        [SerializeField, Tooltip("[Can Be Null] Yes Option Button"), CanBeNull]
        private ButtonAndText _yesButton;

        [SerializeField, Tooltip("[Can Be Null] No Option Button"), CanBeNull]
        private ButtonAndText _noButton;

        [SerializeField, Tooltip("[Can Be Null] Cancel Option Button"), CanBeNull]
        private ButtonAndText _cancelButton;

       

        private void Awake()
        {
            _yesButton?.AddListener(Close);
            _noButton?.AddListener(Close);
            _cancelButton?.AddListener(Close);
            if (_closeButton) _closeButton.onClick.AddListener(Close);
        }

        public void Init(string title, string description, IButtonSettings yesSetting, IButtonSettings noSetting, IButtonSettings cancelSetting)
        {
            Activate(-1);

            _title.SetText(title);
            _description.SetText(description);
            _yesButton?.Set(yesSetting);
            _noButton?.Set(noSetting);
            _cancelButton?.Set(cancelSetting);
            if (_closeButton) _closeButton.onClick.AddListener(cancelSetting.Callback);
        }
    }
}