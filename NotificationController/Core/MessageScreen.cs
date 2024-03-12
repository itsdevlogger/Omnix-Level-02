using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Omnix.Notification
{
    /// <summary> A class for message screen </summary>
    public class MessageScreen : BaseNotificationScreen
    {
        [SerializeField, CanBeNull] private TextMeshProUGUI detailsText;
        [SerializeField] public ButtonAndText button;

        private void Awake()
        {
            button.defaultText = button.textMesh.text;
        }

        internal void Init(string title, string details, BaseButtonConfigs buttonConfig, float autoHideDuration)
        {
            destroyOnHide = false;

            if (titleText != null) titleText.text = title;
            if (detailsText != null) detailsText.text = details;
            buttonConfig.Config(button);
            Activate(this);
            if (autoHideDuration > 0) Close(autoHideDuration);
        }
        
        public MessageScreen Title(string value)
        {
            if (titleText != null) titleText.text = value;
            return this;
        }

        public MessageScreen Details(string value)
        {
            if (detailsText != null) detailsText.text = value;
            return this;
        }

        public MessageScreen ButtonActive(bool value)
        {
            button.button.gameObject.SetActive(value);
            return this;
        }
        
        public MessageScreen OnButton(UnityAction callback)
        {
            button.button.onClick.AddListener(callback);
            return this;
        }
        
        public MessageScreen ButtonText(string value)
        {
            button.textMesh.text = value;
            return this;
        }
    }
}