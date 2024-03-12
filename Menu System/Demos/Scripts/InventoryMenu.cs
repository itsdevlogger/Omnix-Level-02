using System.Collections.Generic;
using MenuManagement.Base;
using MenuManagement.Behaviours;
using UnityEngine;

namespace MenuManagement.Demos
{
    public class InventoryMenu : BaseDynamicMenu<InventoryItem, InventoryMenuItemPrefab>
    {
        [SerializeField] private List<InventoryItem> infos;
        [SerializeField] private InventoryMenu[] otherMenus;
        [SerializeField] private RectTransform selectedOption;
        [SerializeField] private Vector2 selectedAnchored;

        // protected override InventoryMenu CommonObject => this;

        protected override IEnumerable<InventoryItem> GetItems()
        {
            return infos;
        }

        [FilterFunction("Evens")]
        protected override bool Filter1(InventoryItem data)
        {
            return data.count % 2 == 0;
        }
        
        [FilterFunction("Odds", "All the items with odd count")]
        protected override bool Filter2(InventoryItem data)
        {
            return data.count % 2 == 1;
        }

        public void SendTo(int menuIndex)
        {
            if (selectedOption != null)
            {
                selectedOption.gameObject.SetActive(false);
                selectedOption.SetParent(transform);
            }

            if (Selected.ItemData == null)
            {
                return;
            }

            infos.Remove(Selected.ItemData);
            if (Status == MenuStatus.Loaded)
                OnRefresh();

            InventoryMenu otherMenu = otherMenus[menuIndex];
            otherMenu.infos.Add(Selected.ItemData);
            if (otherMenu.Status == MenuStatus.Loaded)
                otherMenu.OnRefresh();
        }

        protected override void OnItemSelected(InventoryItem data, InventoryMenuItemPrefab item)
        {
            if (selectedOption != null)
            {
                selectedOption.SetParent(item.transform);
                selectedOption.anchoredPosition = selectedAnchored;
                selectedOption.gameObject.SetActive(true);
            }
        }

        protected override void OnItemDeselected(InventoryItem data, InventoryMenuItemPrefab item)
        {
            if (selectedOption != null)
            {
                selectedOption.gameObject.SetActive(false);
                selectedOption.SetParent(transform);
            }
        }
    }
}