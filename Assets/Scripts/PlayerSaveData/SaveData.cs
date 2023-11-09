using System;
using System.Collections.Generic;
using Items;

namespace PlayerSaveData
{
    [Serializable]
    public class SaveData
    {
        public int currentCurrency;
        public List<string> obtainedItems;
        public int totalCropValue;
        public EquippedItemsSaveData equippedItems;

        public string EquippedHair => equippedItems.equippedHair;
        public string EquippedOutfit => equippedItems.equippedOutfit;
        public string EquippedHat => equippedItems.equippedHat;

        public SaveData()
        {
            currentCurrency = 0;
            obtainedItems = new();
            totalCropValue = 0;
            equippedItems = new();
        }

        public void UpdateCurrency(int amount)
        {
            currentCurrency = amount;
        }

        public void ObtainItem(string itemId)
        {
            obtainedItems.Add(itemId);
        }

        public void UpdateCropValue(int value)
        {
            totalCropValue = value;
        }

        public void RemoveItem(string itemId)
        {
            if (!obtainedItems.Contains(itemId)) return;
            obtainedItems.Remove(itemId);
        }

        public void ResetCropValue()
        {
            totalCropValue = 0;
        }

        public void EquipItem(ItemData itemData)
        {
            switch (itemData.itemType)
            {
                case ItemType.Hair:
                    equippedItems.equippedHair = itemData.itemId;
                    break;
                case ItemType.Outfit:
                    equippedItems.equippedOutfit = itemData.itemId;
                    break;
                case ItemType.Hat:
                    equippedItems.equippedHat = itemData.itemId;
                    break;
                case ItemType.None:
                case ItemType.Crop:
                default:
                    break;
            }
        }

        public void UnequipItem(ItemType itemType)
        {
            switch (itemType)
            {
                case ItemType.Hair:
                    equippedItems.equippedHair = string.Empty;
                    break;
                case ItemType.Outfit:
                    equippedItems.equippedOutfit = string.Empty;
                    break;
                case ItemType.Hat:
                    equippedItems.equippedHat = string.Empty;
                    break;
                case ItemType.None:
                case ItemType.Crop:
                default:
                    break;
            }
        }
    }

    [Serializable]
    public class EquippedItemsSaveData
    {
        public string equippedHair;
        public string equippedOutfit;
        public string equippedHat;
    }
}