using UnityEngine;
using UnityEngine.UI;

namespace Omnix.Callbacks
{
    [RequireComponent(typeof(Slider))]
    public abstract class SliderCallback : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            Slider toggle = GetComponent<Slider>();
            if (toggle)
                toggle.onValueChanged.AddListener(OnValueChanged);
        }

        protected virtual void OnDisable()
        {
            Slider toggle = GetComponent<Slider>();
            if (toggle)
                toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        protected abstract void OnValueChanged(float newValue);
    }
}