using UnityEngine;
using UnityEngine.UI;

namespace Omnix.Callbacks
{
    [RequireComponent(typeof(InputField))]
    public abstract class InputFieldCallback : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            InputField toggle = GetComponent<InputField>();
            if (toggle)
                toggle.onValueChanged.AddListener(OnValueChanged);
        }

        protected virtual void OnDisable()
        {
            InputField toggle = GetComponent<InputField>();
            if (toggle)
                toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        protected abstract void OnValueChanged(string newValue);
    }
}