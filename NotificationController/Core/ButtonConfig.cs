using System;
using UnityEngine.Events;

namespace Omnix.Notification
{
    public abstract class BaseButtonConfigs
    {
        protected bool isActive = true;
        protected string text;

        public void Config(ButtonAndText target)
        {
            target.button.gameObject.SetActive(isActive);
            if (isActive == false) return;

            if (target.button != null)
            {
                target.button.onClick.AddListener(() =>
                {
                    Notification.CloseActiveScreen();
                    Callback();
                });
            }

            if (target.textMesh != null)
            {
                if (text == null) target.textMesh.text = target.defaultText;
                else target.textMesh.text = text;
            }
        }
        
        protected abstract void Callback();
    }
    
    internal class ButtonConfig : BaseButtonConfigs
    {
        public static readonly ButtonConfig Inactive = new ButtonConfig(null, null) { isActive = false };
        private readonly UnityAction callback;

        public ButtonConfig(string text, UnityAction callback)
        {
            this.text = text;
            this.callback = callback;
            this.isActive = true;
        }

        protected override void Callback()
        {
            callback?.Invoke();
        }
    }
    
    [Serializable]
    public class ButtonConfigSerializable : BaseButtonConfigs
    {
        public UnityEvent callback;

        public ButtonConfigSerializable(string text)
        {
            this.text = text;
            this.isActive = true;
        }
        
        protected override void Callback()
        {
            callback?.Invoke();
        }
    }
}