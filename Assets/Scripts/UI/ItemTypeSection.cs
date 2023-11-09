using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using JetBrains.Annotations;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace UI
{
    public class ItemTypeSection : MonoBehaviour
    {
        public static event Action<ItemView, ItemData> OnItemViewSelected; 
        
        private static List<ItemData> _allItems;
        private List<ItemData> _loadedItems;

        public ItemType ItemType => itemType;
        
        [SerializeField] private ItemType itemType;
        [SerializeField] private bool isShop;
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private List<ItemView> itemViewList;

        private ItemView _noneItemView;
        public void Init()
        {
            if (itemType == ItemType.None)
            {
                Destroy(this.gameObject);
                return;
            }

            Setup();
        }

        private void Setup()
        {
            LoadAllItems();
            _loadedItems = GetRelevantItems();

            if (isShop)
            {
                SetupAllItemViewsShopkeeper();
                CheckObtainedItems();
            } else
            {
                SetupAllItemViewsInventory();
                CheckEquippedItems();
                
            }
            SetupText();
        }

        public void CheckEquippedItems()
        {
            var item = InventoryManager.GetEquippedItem(itemType);
            foreach (var itemView in itemViewList)
            {
                itemView.skipLockCheck = false;
                itemView.UpdateVisuals();
                itemView.SetSelected(itemView.IsItem(item));
            }
        }
        
        public void CheckObtainedItems()
        {
            foreach (var itemView in itemViewList)
            {
                var hasItem = InventoryManager.HasItem(itemView.itemId);
                itemView.SetObtained(hasItem);
            }
        }

        private void SetupText()
        {
            var title = Enum.GetName((typeof(ItemType)), itemType);
            titleText.SetText($"~{title}~");
        }

        private List<ItemData> GetRelevantItems() => _allItems.FindAll(i => i.itemType == itemType);

        private void LoadAllItems()
        {
            if (_allItems == null || _allItems.Count == 0)
            {
                _allItems = InventoryManager.AllItems;
            }
        }

        private void LoadObtainedItems()
        {
            if (_allItems == null || _allItems.Count == 0)
            {
                _allItems = InventoryManager.ObtainedItems;
            }
        }

        private void SetupAllItemViewsInventory()
        {
            for (var i = 0; i < itemViewList.Count; i++)
            {
                var itemView = itemViewList[i];
                var hasItem = _loadedItems.Count > i - 1;

                if (i == 0)
                {
                    itemView.OnSelected += OnInventoryItemViewSelected;
                    itemView.SetNone();
                    _noneItemView = itemView;
                    continue;
                }
                
                if (!hasItem)
                {
                    itemView.Disable();
                    continue;
                }
                itemView.OnSelected += OnInventoryItemViewSelected;
                itemView.SetItem(_loadedItems[i - 1]);
            }
        }

        private void SetupAllItemViewsShopkeeper()
        {
            for (var i = 0; i < itemViewList.Count; i++)
            {
                var itemView = itemViewList[i];
                var hasItem = _loadedItems.Count > i;

                if (!hasItem)
                {
                    itemView.Disable();
                    continue;
                }
                itemView.OnSelected += OnShopkeeperItemViewSelected;
                itemView.skipLockCheck = true;
                itemView.SetItem(_loadedItems[i]);
            }
        }

        public void DeselectAll()
        {
            foreach (var itemView in itemViewList.Where(i => i.isSelected))
            {
                itemView.SetSelected(false);
            }

            if (isShop)
            {
                CheckObtainedItems();
            }
            else
            {
                CheckEquippedItems();
            }
        }
        
        [CanBeNull]
        private void OnShopkeeperItemViewSelected(ItemView itemView, ItemData itemData)
        {
            OnItemViewSelected?.Invoke(itemView, itemData);
        }

        [CanBeNull]
        private void OnInventoryItemViewSelected(ItemView itemView, ItemData itemData)
        {
            if (itemData == null)
            {
                InventoryManager.Unequip(itemType);
            }
            else
            {
                InventoryManager.Equip(itemData);
            }

            CheckEquippedItems();
        }

        public void UnlockItem(string itemId)
        {
            foreach (var itemView in itemViewList)
            {
                if (itemView.itemId != itemId) continue;
                itemView.Unlock();
            }
        }
    }
}
