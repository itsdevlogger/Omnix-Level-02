using System;
using UnityEngine;
using UnityEngine.Events;

namespace Omnix.Notification
{
    public interface IButtonSettings
    {
        public string Text { get; }
        public bool IsActive { get; }
        void Callback();
    }
    
    [Serializable]
    public class ButtonSettingsSerializable : IButtonSettings
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public bool IsActive { get; set; }
        public UnityEvent onClick;
        public void Callback() => onClick?.Invoke();
    }
    
    public struct ButtonSettings : IButtonSettings
    {
        public string Text { get; }
        public bool IsActive { get; }
        public Action onClick;

        public ButtonSettings(string text, bool isActive, Action callback)
        {
            Text = text;
            onClick = callback;
            IsActive = isActive;
        }
        
        public ButtonSettings(string text, Action callback)
        {
            Text = text;
            onClick = callback;
            IsActive = true;
        }

        public ButtonSettings(bool isActive)
        {
            Text = null;
            onClick = null;
            IsActive = true;
        }

        public void Callback() => onClick?.Invoke();
    }
}