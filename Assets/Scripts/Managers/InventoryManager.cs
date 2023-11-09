using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Items;
using PlayerSaveData;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class InventoryManager : MonoSingleton<InventoryManager>
    {
        public static List<ItemData> AllItems
        {
            get
            {
                if (_allItems != null) return _allItems;
                _allItems = Resources.LoadAll<ItemData>(GlobalVariables.ItemDataResourcesEndpoint).ToList();
                SetupAllItems();
                return _allItems;
            }
        }

        public static List<ItemData> ObtainedItems
        {
            get
            {
                if (_obtainedItems != null) return _obtainedItems;
                _obtainedItems = GameManager.Instance.GetObtainedItems();
                return _obtainedItems;
            }
            set => _obtainedItems = value;
        }

        public static ItemData EquippedHair => GameManager.Instance.PlayerController.EquippedHair;
        public static ItemData EquippedOutfit => GameManager.Instance.PlayerController.EquippedOutfit;
        public static ItemData EquippedHat => GameManager.Instance.PlayerController.EquippedHat;

        private static void SetupAllItems()
        {
            _allItems.ForEach(i => i.ResetVisuals());
        }

        private static List<ItemData> _allItems;
        private static List<ItemData> _obtainedItems;
        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            InputManager.OnInputDown += CheckInput;
            GameManager.OnItemObtained += UpdateObtainedItems;
        }

        private void UpdateObtainedItems(string itemId)
        {
            _obtainedItems.Add(GlobalVariables.GetItemData(itemId));
            ObtainedItems = _obtainedItems;
        }

        private void Unsubscribe()
        {
            InputManager.OnInputDown -= CheckInput;
            GameManager.OnItemObtained -= UpdateObtainedItems;
        }

        private void CheckInput(PlayerAction action)
        {
            if (action != PlayerAction.Inventory) return;
            UIManager.Instance.ToggleInventory();
        }

        public static bool HasItem(ItemData itemData)
        {
            return HasItem(itemData.itemId);
        }

        public static bool HasItem(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && ObtainedItems.Any(i => i.itemId == itemId);
        }

        public static void Equip(ItemData itemData)
        {
            GameManager.Equip(itemData);
        }

        public static void Unequip(ItemType itemType)
        {
            GameManager.Unequip(itemType);
        }

        public static ItemData GetEquippedItem(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Hair:
                    return EquippedHair;
                case ItemType.Outfit:
                    return EquippedOutfit;
                case ItemType.Hat:
                    return EquippedHat;
                case ItemType.None:
                default:
                    Debug.LogError($"Invalid ItemType: {itemType}.");
                    return null;
            }
        }

        public static void ObtainCrop(ItemData item)
        {
            GameManager.ObtainCrop(item.itemCost);
        }
    }
}
