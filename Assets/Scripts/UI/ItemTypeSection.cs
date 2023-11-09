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
        private static List<ItemData> _allItems;
        private List<ItemData> _loadedItems;
        
        [SerializeField] private ItemType itemType;
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
            SetupAllItemViews();
            SetupText();
            CheckEquippedItems();
        }

        private void CheckEquippedItems()
        {
            var item = InventoryManager.GetEquippedItem(itemType);
            foreach (var itemView in itemViewList)
            {
                itemView.SetSelected(itemView.IsItem(item));
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

        private void SetupAllItemViews()
        {
            for (var i = 0; i < itemViewList.Count; i++)
            {
                var itemView = itemViewList[i];
                var hasItem = _loadedItems.Count > i - 1;

                if (i == 0)
                {
                    itemView.OnSelected += OnItemViewSelected;
                    itemView.SetNone();
                    _noneItemView = itemView;
                    continue;
                }
                
                if (!hasItem)
                {
                    itemView.Disable();
                    continue;
                }
                itemView.OnSelected += OnItemViewSelected;
                itemView.SetItem(_loadedItems[i - 1]);
            }
        }

        [CanBeNull]
        private void OnItemViewSelected(ItemData itemData)
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
    }
}
