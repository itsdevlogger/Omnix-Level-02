using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Omnix.Monos
{
    public class OnKeyEvent : MonoBehaviour
    {
        public enum CheckMode
        {
            KeyDown,
            KeyHold,
            KeyUp
        }

        [SerializeField] private CheckMode checkMode;
        [SerializeField] private KeyCode keycode;
        [SerializeField] private UnityEvent onPress;



        private Func<KeyCode, bool> checkFunc;
        private void Start()
        {
            checkFunc = checkMode switch
            {
                CheckMode.KeyDown => Input.GetKeyDown,
                CheckMode.KeyHold => Input.GetKey,
                CheckMode.KeyUp => Input.GetKeyUp,
                _ => checkFunc
            };
        }

        void Update()
        {
            if (checkFunc.Invoke(keycode))
            {
                onPress?.Invoke();
                if (TryGetComponent(out IKeyEventListener listener))
                {
                    listener.OnKeyEvent(keycode, checkMode);
                }                
            }
        }
    }


    public interface IKeyEventListener
    {
        public void OnKeyEvent(KeyCode code, OnKeyEvent.CheckMode checkMode);
    }
}