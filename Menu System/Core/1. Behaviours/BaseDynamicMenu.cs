using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using MenuManagement.Base;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MenuManagement.Behaviours
{
    public abstract class BaseDynamicMenu<TData, TUiComponent, TCommonObject> : BaseDynamicMenu<TData, TUiComponent>
        where TUiComponent : BaseItemPrefab<TData, TCommonObject>
    {
        /// <summary> Get the common object shared by both Menu and Item </summary>
        protected abstract TCommonObject CommonObject { get; }

        protected override void SetupItem(TUiComponent instance, TData data)
        {
            instance.SetupSelf(this, CommonObject, data);
        }
    }
    
    /// <summary>
    /// Base Class for the menu which is dynamically created.
    /// This menu is used to display some type of data to the user, and get some commands from them.
    /// </summary>
    /// <typeparam name="TData"> Data container type (serializable) </typeparam>
    /// <typeparam name="TUiComponent"> UI prefab type </typeparam>
    /// <typeparam name="TCommonObject"> Type of any object that both the menu and prefab will be using </typeparam>
    [GroupEvents("onItemSelected", "onItemDeselected", "onItemConfirmed")]
    [GroupRuntimeConstant("activateOnError", "deactivateOnError")]
    public abstract class BaseDynamicMenu<TData, TUiComponent> : BaseMenu, IDynamicMenu<TData>
        where TUiComponent : BaseItemPrefab<TData>
    {
        #region Class
        [Serializable]
        public class SelectedInfo
        {
            [CanBeNull, Tooltip("Can Be Null. UI Element to show info of selected item.")]
            public TUiComponent previewItem;

            [CanBeNull, Tooltip("Can Be Null. Data to show in selectedInfo when nothing is selected")]
            public TData previewDefaultVales;

            public TData ItemData { get; internal set; }
            public TUiComponent Item { get; internal set; }
        }
        #endregion

        #region Fields
        // @formatter:off
        [SerializeField, Tooltip("If false, then child-item wont be deselected unless another child-item is selected.")] private bool allowDeselect;
        [SerializeField, Tooltip("Choose the behaviour for this menu")]                                                  private DynamicMenuBehaviour behaviour;
        [SerializeField, Tooltip("Choose the filter for this menu")]                                                     private int filterIndex;
        
        [SerializeField, Tooltip("Prefab that represent the data in child")]                                             protected TUiComponent itemPrefab;
        [SerializeField, Tooltip("Parent gameObject for all the items")]                                                 protected Transform itemsParent;
        [SerializeField, Tooltip("Can Be Null. When the user clicks this button OnItemClickedMethod will be called.")]   protected Button confirmButton;

        [SerializeField, Tooltip("GameObejcts that should be activated if menu fails to load.")]                         protected GameObject[] activateOnError;
        [SerializeField, Tooltip("GameObejcts that should be deactivated if menu fails to load.")]                       protected GameObject[] deactivateOnError;


        [Tooltip("Callback for when an item is selected")]                                                               public UnityEvent<TData, TUiComponent> onItemSelected;
        [Tooltip("Callback for when an item is deselected")]                                                             public UnityEvent<TData, TUiComponent> onItemDeselected;
        [Tooltip("Callback for when an item is confirmed")]                                                              public UnityEvent<TData, TUiComponent> onItemConfirmed;
        // @formatter:on

        [field: SerializeField] public SelectedInfo Selected { get; private set; }
        private Dictionary<TData, TUiComponent> Instances;
        public bool AllowDeselect => allowDeselect;

        public DynamicMenuBehaviour Behaviour
        {
            get => behaviour;
            set
            {
                if (Application.isPlaying) Debug.LogError("Menu Behaviour is a runtime constant. Set this in editor or in MonoBehavior.Reset method");
                else behaviour = value;
            }
        }
        #endregion

        #region Abstract
        /// <summary> Loop through of all the possible elements may need to be displayed in the UI. You can check if a particular element can be displayed in <see cref="ShouldDisplayItem"/> </summary>
        protected abstract IEnumerable<TData> GetItems();
        #endregion

        #region Virtual Members
        protected virtual bool Filter1(TData data) => true;
        protected virtual bool Filter2(TData data) => true;
        protected virtual bool Filter3(TData data) => true;
        protected virtual bool Filter4(TData data) => true;
        protected virtual bool Filter5(TData data) => true;
        protected virtual bool Filter6(TData data) => true;
        protected virtual bool Filter7(TData data) => true;
        protected virtual bool Filter8(TData data) => true;
        protected virtual bool Filter9(TData data) => true;
        protected virtual bool Filter10(TData data) => true;

        /// <summary>
        /// Any method on child class that has <see cref="FilterFunctionAttribute"/>, takes in TData as parameter and returns bool, will appear as a filter option in the inspector.
        /// </summary>
        /// <returns> user selected filter </returns>
        private Func<TData, bool> GetFilterMethod()
        {
            return filterIndex switch
            {
                0 => (_ => true),
                1 => Filter1,
                2 => Filter2,
                3 => Filter3,
                4 => Filter4,
                5 => Filter5,
                6 => Filter6,
                7 => Filter7,
                8 => Filter8,
                9 => Filter9,
                10 => Filter10,
                _ => (_ => true),
            };
        }

        /// <summary> Called when the menu fails to loading or no item is spawned </summary>
        protected virtual void OnLoadingFailed()
        {
        }

        protected virtual void OnItemSelected(TData data, TUiComponent item)
        {
        }

        protected virtual void OnItemDeselected(TData data, TUiComponent item)
        {
        }

        protected virtual void OnItemConfirmed(TData data, TUiComponent item)
        {
        }

        protected virtual void SetupItem(TUiComponent instance, TData data)
        {
            instance.SetupSelf(this, data);
        }
        #endregion

        #region Overrides
        protected override void Initialize()
        {
            if (Selected.previewItem != null) Selected.previewItem.SetBindings();

            if (behaviour == DynamicMenuBehaviour.NotInteractable) return;

            if (behaviour == DynamicMenuBehaviour.PeekIn && confirmButton != null)
            {
                confirmButton.onClick.AddListener(() => ConfirmItem(Selected.ItemData, Selected.Item));
            }

            onItemSelected.AddListener(OnItemSelected);
            onItemDeselected.AddListener(OnItemDeselected);
            onItemConfirmed.AddListener(OnItemConfirmed);
        }

        protected override void OnLoad()
        {
            LoadInternal(GetItems());
        }

        protected override void OnUnload()
        {
            if (Selected.previewItem)
            {
                SetupItem(Selected.previewItem, Selected.previewDefaultVales);
            }
            DestroyAllInstances();
        }

        protected override void OnRefresh()
        {
            OnUnload();
            LoadInternal(GetItems());
        }
        #endregion

        #region Functionality
        private void DestroyAllInstances()
        {
            if (Instances == null)
            {
                Instances = new Dictionary<TData, TUiComponent>();
                return;
            }

            foreach (KeyValuePair<TData, TUiComponent> instance in Instances)
            {
                if (instance.Value)
                    Destroy(instance.Value.gameObject);
            }

            Instances.Clear();
        }
        
        private void LoadInternal(IEnumerable<TData> collection)
        {
            DestroyAllInstances();

            TUiComponent itemToSelect = null;
            TData dataToSelect = default;
            bool loadingFailed = true;
            Func<TData, bool> filter = GetFilterMethod();

            SetError(false);

            // Create instances from provided data
            if (collection != null)
            {
                foreach (TData data in collection)
                {
                    if (filter(data) == false) continue;

                    if (Instances.ContainsKey(data))
                    {
                        Debug.LogError($"The provided item list has duplicate entries. Menu of Type {GetType()} attached to GameObject {gameObject.name}.");
                        continue;
                    }

                    TUiComponent item = Instantiate(itemPrefab, itemsParent);
                    item.SetBindings();

                    // if the item is first item that we are spawning, then it should be on, otherwise off.
                    if (loadingFailed)
                    {
                        itemToSelect = item;
                        dataToSelect = data;
                    }

                    SetupItem(item, data);
                    Instances.Add(data, item);
                    loadingFailed = false;
                }
            }

            // If previously selected item is present in this itteration, select it
            if (Selected.ItemData != null && Instances.TryGetValue(Selected.ItemData, out TUiComponent temp))
            {
                itemToSelect = temp;
                dataToSelect = Selected.ItemData;
            }


            // OnLoadingFailed is called when there are no elements in the menu
            if (loadingFailed)
            {
                if (Selected.previewItem)
                {
                    SetupItem(Selected.previewItem, Selected.previewDefaultVales);
                }

                SetError(true);
                OnLoadingFailed();
            }
            else if (allowDeselect == false)
            {
                if (behaviour != DynamicMenuBehaviour.NotInteractable && itemToSelect != null)
                {
                    SelectItemInternal(dataToSelect, itemToSelect);
                }
                else if (Selected.previewItem)
                {
                    SetupItem(Selected.previewItem, Selected.previewDefaultVales);
                }
            }
        }
        
        private void SelectItemInternal(TData data, TUiComponent item)
        {
            if (item == Selected.previewItem) return;
            if (Selected.Item != null)
            {
                onItemDeselected?.Invoke(Selected.ItemData, Selected.Item);
                Selected.Item.OnDeselect();
            }

            Selected.Item = item;
            Selected.ItemData = data;
            UpdateChildToggles();
            if (Selected.previewItem) SetupItem(Selected.previewItem, data);

            item.OnSelect();
            onItemSelected?.Invoke(data, item);
        }

        
        private void SetError(bool value)
        {
            foreach (var item in activateOnError)
            {
                item.SetActive(value);
            }

            value = !value;
            foreach (var item in deactivateOnError)
            {
                item.SetActive(value);
            }
        }

        private void UpdateChildToggles()
        {
            foreach (KeyValuePair<TData, TUiComponent> pair in Instances)
            {
                pair.Value.SetSelectedState(pair.Value == Selected.Item);
            }
        }

        public void SelectItem(TUiComponent item)
        {
            if (item == null)
            {
                Debug.LogError("Item to select is null. If you are trying to Deselect the everything, call DeselectItem button.");
                return;
            }

            foreach (KeyValuePair<TData, TUiComponent> pair in Instances)
            {
                if (pair.Value == item)
                {
                    SelectItemInternal(pair.Key, pair.Value);
                    return;
                }
            }

            Debug.LogError("Item to select is not present in the menu. If you are trying to Deselect the everything, call DeselectItem button.");
        }

        public void SelectItem(TData data)
        {
            if (data == null)
            {
                Debug.LogError("Data to select is null. If you are trying to Deselect the everything, call DeselectItem button.");
                return;
            }

            if (Instances.TryGetValue(data, out TUiComponent item) == false)
            {
                Debug.LogError("Data to select is not present in the menu. If you are trying to Deselect the everything, call DeselectItem button.");
                return;
            }

            SelectItemInternal(data, item);
        }

        public void DeselectItem()
        {
            if (allowDeselect)
            {
                onItemDeselected?.Invoke(Selected.ItemData, Selected.Item);
                Selected.Item = null;
                Selected.ItemData = default;
                if (Selected.previewItem) SetupItem(Selected.previewItem, Selected.previewDefaultVales);
                UpdateChildToggles();
            }
        }

        public void ConfirmItem(TData data)
        {
            if (Instances.TryGetValue(data, out TUiComponent instance))
            {
                ConfirmItem(data, instance);
            }
        }

        public void ConfirmItem(TData data, TUiComponent item)
        {
            if (Selected.Item != item) SelectItemInternal(data, item);
            onItemConfirmed?.Invoke(data, item);
        }

        public void SortBy(Comparison<TData> comparison)
        {
            var allKeys = new List<TData>();

            foreach (KeyValuePair<TData, TUiComponent> pair in Instances)
            {
                allKeys.Add(pair.Key);
            }

            allKeys.Sort(comparison);
            OnUnload();
            LoadInternal(allKeys);
        }

        public TUiComponent GetUiComponent(TData data)
        {
            if (Instances.TryGetValue(data, out TUiComponent instance)) return instance;
            return null;
        }
        #endregion
    }
}