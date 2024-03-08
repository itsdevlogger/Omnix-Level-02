using UnityEngine;
using UnityEngine.UI;

namespace Omnix.Callbacks
{
    [RequireComponent(typeof(Scrollbar))]
    public abstract class ScrollbarCallback : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            Scrollbar toggle = GetComponent<Scrollbar>();
            if (toggle)
                toggle.onValueChanged.AddListener(OnValueChanged);
        }

        protected virtual void OnDisable()
        {
            Scrollbar toggle = GetComponent<Scrollbar>();
            if (toggle)
                toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        protected abstract void OnValueChanged(float newValue);
    }
}