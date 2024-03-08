using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Omnix.Callbacks
{
    [RequireComponent(typeof(Toggle))]
    public class ToggleCallbacks : MonoBehaviour
    {
        [SerializeField] private UnityEvent OnTurnedOnEvent;
        [SerializeField] private UnityEvent OnTurnedOffEvent;
        protected virtual void OnEnable()
        {
            Toggle toggle = GetComponent<Toggle>();
            if (toggle)
                toggle.onValueChanged.AddListener(OnValueChangedInner);
        }

        protected virtual void OnDisable()
        {
            Toggle toggle = GetComponent<Toggle>();
            if (toggle)
                toggle.onValueChanged.RemoveListener(OnValueChangedInner);
        }


        private void OnValueChangedInner(bool newValue)
        {
            if (newValue == true)
            {
                OnTurnedOn();
                OnTurnedOnEvent?.Invoke();
            }
            else
            {
                OnTurnedOff();
                OnTurnedOffEvent?.Invoke();
            }
            OnValueChanged(newValue);
        }
        protected virtual void OnValueChanged(bool newValue) { }
        protected virtual void OnTurnedOn() { }
        protected virtual void OnTurnedOff() { }
    }
}