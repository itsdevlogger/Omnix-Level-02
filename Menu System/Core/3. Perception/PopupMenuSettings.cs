using System;
using MenuManagement.Base;
using UnityEngine;

namespace MenuManagement.Perception
{
    /// <summary> Converts a Menu into a popup menu. </summary>
    [RequireComponent(typeof(BaseMenu))]
    public class PopupMenuSettings : MonoBehaviour
    {
        private enum Behaviour
        {
            PressToToggle,
            ActiveWhilePressed
        }
        
        private enum TriggerType
        {
            Button = 0,
            KeyCode = 1
        }
        
        
        [SerializeField] private Behaviour behaviour;
        [SerializeField] private TriggerType triggerType;
        
        [SerializeField, HideInInspector] private string activationButton;
        [SerializeField, HideInInspector] private KeyCode activationKeycode;
        
        private BaseMenu menu;

        private void Awake()
        {
            menu = GetComponent<BaseMenu>();
            PopupMenuManger.Instance.Add(this);
        }

        private void OnDestroy()
        {
            PopupMenuManger.Instance.Remove(this);
        }

        private bool GetTriggerDown()
        {
            return triggerType switch
            {
                TriggerType.Button => Input.GetButtonDown(activationButton),
                TriggerType.KeyCode => Input.GetKeyDown(activationKeycode),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private bool GetTriggerUp()
        {
            return triggerType switch
            {
                TriggerType.Button => Input.GetButtonUp(activationButton),
                TriggerType.KeyCode => Input.GetKeyUp(activationKeycode),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        private bool ShouldLoad()
        {
            return behaviour switch
            {
                Behaviour.PressToToggle => GetTriggerUp(),
                Behaviour.ActiveWhilePressed => GetTriggerDown(),
                _ => false
            };
        }

        private bool ShouldUnload()
        {
            return GetTriggerUp();
        }
        
        internal void UpdateCallback()
        {
            switch (menu.Status)
            {
                case MenuStatus.Unloaded when ShouldLoad():
                {
                    MenuLoader.Load(menu);
                    break;
                }
                case MenuStatus.Loaded when ShouldUnload():
                {
                    MenuLoader.Unload(menu);
                    break;
                }
            }
        }
    }
}