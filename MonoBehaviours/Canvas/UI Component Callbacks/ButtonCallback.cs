using UnityEngine;
using UnityEngine.UI;

namespace Omnix.Callbacks
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonCallback : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            Button toggle = GetComponent<Button>();
            if (toggle)
                toggle.onClick.AddListener(OnClick);
        }

        protected virtual void OnDisable()
        {
            Button toggle = GetComponent<Button>();
            if (toggle)
                toggle.onClick.RemoveListener(OnClick);
        }

        protected abstract void OnClick();
    }
}