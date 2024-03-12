using System;
using MenuManagement.Base;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MenuManagement.Behaviours
{
    public abstract class BaseItemPrefab<TData, TCommonObject> : BaseItemPrefab<TData>
    {
        public TCommonObject CommonObject { get; private set; }

        internal void SetupSelf(IDynamicMenu<TData> menu, TCommonObject commonObject, TData data)
        {
            CommonObject = commonObject;
            SetupSelf(menu, data);
        }
    }
    
    public abstract class BaseItemPrefab<TData> : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
    {
        [SerializeField, Tooltip("Can Be Null. Toggle that represents state of this menu item.")] protected Toggle toggle;
        
        private IDynamicMenu<TData> _parentMenu;
        private TData _myData;
        public bool IsSelected { get; private set; }
        
        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            if (_parentMenu.Behaviour == DynamicMenuBehaviour.QuickSelect)
            {
                _parentMenu.SelectItem(_myData);
            }
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            if (_parentMenu.Behaviour == DynamicMenuBehaviour.QuickSelect)
            {
                _parentMenu.DeselectItem();
            }
        }

        void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
        {
            switch (_parentMenu.Behaviour)
            {
                case DynamicMenuBehaviour.NotInteractable:
                    break;
                case DynamicMenuBehaviour.PeekIn:
                    if (IsSelected) _parentMenu.DeselectItem();
                    else _parentMenu.SelectItem(_myData);
                    break;
                case DynamicMenuBehaviour.QuickSelect:
                    _parentMenu.ConfirmItem(_myData);
                    break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary> Setup this item in UI </summary>
        /// <remarks> 
        /// This method can be called multiple times on same item. Thus do not set binding (AddListener) to any of the child component of this object in this method. 
        /// Use <see cref="SetBindings"/> instead. 
        /// </remarks>
        /// <param name="data"></param>
        protected virtual void OnSetup(TData data)
        {
        }

        internal void SetupSelf(IDynamicMenu<TData> menu, TData data)
        {
            _parentMenu = menu;
            _myData = data;
            IsSelected = false;
            if (toggle)
            {
                toggle.isOn = false;
                toggle.interactable = menu.Behaviour != DynamicMenuBehaviour.NotInteractable;
            }
            
            OnSetup(data);
        }

        internal void SetSelectedState(bool state)
        {
            IsSelected = state;
            if (toggle) toggle.isOn = state;
        }
        
        /// <summary> Setup the bindings (AddListener) for this item. </summary>
        /// <remarks> The data can change, prefer global callback listeners </remarks>
        public virtual void SetBindings()
        {

        }
        
        public virtual void OnSelect()
        {
            IsSelected = true;
            if (toggle) toggle.isOn = true;
        }

        public virtual void OnDeselect()
        {
            IsSelected = false;
            if (toggle) toggle.isOn = false;
        }

        public virtual void OnConfirm()
        {
        }
    }
}