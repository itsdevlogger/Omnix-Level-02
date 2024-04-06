using System;
using JetBrains.Annotations;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Omnix.Notification
{
    [Serializable]
    public class ButtonAndText
    {
        [SerializeField, CanBeNull] private Button _button;
        [SerializeField, CanBeNull] private TextMeshProUGUI _text;
        [SerializeField] private string _defaultText;

        public void Set(IButtonSettings settings)
        {
            if (_button)
            {
                _button.gameObject.SetActive(settings.IsActive);
                _button.onClick.AddListener(settings.Callback);
            }

            if (_text)
            {
                _text.gameObject.SetActive(settings.IsActive);
                _text.SetText(string.IsNullOrEmpty(settings.Text) ? _defaultText : settings.Text);
            }
        }

        public void AddListener(UnityAction action)
        {
            if (_button)
            {
                _button.onClick.AddListener(action);
            }
        }
    }
}