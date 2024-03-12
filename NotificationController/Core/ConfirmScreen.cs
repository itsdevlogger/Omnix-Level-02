using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Omnix.Notification
{
    /// <summary>
    /// A class for confirm screen
    /// </summary>
    public class ConfirmScreen : BaseNotificationScreen
    {
        [SerializeField, CanBeNull] private TextMeshProUGUI detailsText;
        [SerializeField] public ButtonAndText yesButton;
        [SerializeField] public ButtonAndText noButton;
        [SerializeField] public ButtonAndText cancelButton;

        private void Awake()
        {
            yesButton.defaultText = yesButton.textMesh.text;
            noButton.defaultText = noButton.textMesh.text;
            cancelButton.defaultText = cancelButton.textMesh.text;
        }

        internal void Init(string title, string details, BaseButtonConfigs yesConfig, BaseButtonConfigs noConfig, BaseButtonConfigs cancelButtonConfig)
        {
            destroyOnHide = false;

            if (titleText != null) titleText.text = title;
            if (detailsText != null) detailsText.text = details;

            yesConfig.Config(yesButton);
            noConfig.Config(noButton);
            cancelButtonConfig.Config(cancelButton);
            Activate(this);
        }
        
        
        public ConfirmScreen Title(string value)
        {
            if (titleText != null) titleText.text = value;
            return this;
        }

        public ConfirmScreen Details(string value)
        {
            if (detailsText != null) detailsText.text = value;
            return this;
        }

        public ConfirmScreen YesActive(bool value)
        {
            yesButton.button.gameObject.SetActive(value);
            return this;
        }
        
        public ConfirmScreen OnYes(UnityAction callback)
        {
            yesButton.button.onClick.AddListener(callback);
            return this;
        }
        
        public ConfirmScreen YesText(string value)
        {
            yesButton.textMesh.text = value;
            return this;
        }
        
        public ConfirmScreen NoActive(bool value)
        {
            noButton.button.gameObject.SetActive(value);
            return this;
        }
        
        public ConfirmScreen OnNo(UnityAction callback)
        {
            noButton.button.onClick.AddListener(callback);
            return this;
        }
        
        public ConfirmScreen NoText(string value)
        {
            noButton.textMesh.text = value;
            return this;
        }
        
        public ConfirmScreen CancelActive(bool value)
        {
            cancelButton.button.gameObject.SetActive(value);
            return this;
        }
        
        public ConfirmScreen OnCancel(UnityAction callback)
        {
            cancelButton.button.onClick.AddListener(callback);
            return this;
        }
        
        public ConfirmScreen CancelText(string value)
        {
            cancelButton.textMesh.text = value;
            return this;
        }
    }
}