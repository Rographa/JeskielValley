using System;
using System.Collections;
using System.Collections.Generic;
using Items;
using Player;
using Utilities;
using PlayerSaveData;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoSingleton<GameManager>
    {
        public PlayerController PlayerController { get; private set; }
        
        public static Action<int> OnCurrencyChanged;
        public static Action<int> OnCropValueChanged;
        public static Action<string> OnItemObtained;
        public static Action<string> OnItemRemoved;
        public static Action<ItemData> OnItemEquipped;
        public static Action<ItemType> OnItemUnequipped;

        public static int CropValue => Instance._currentSaveData.totalCropValue;
        public static int CurrentCurrency => Instance._currentSaveData.currentCurrency;

        public static string EquippedHair => Instance._currentSaveData.EquippedHair;
        public static string EquippedOutfit => Instance._currentSaveData.EquippedOutfit;
        public static string EquippedHat => Instance._currentSaveData.EquippedHat;

        [SerializeField] private bool autoSave = true;
        [SerializeField] private float autoSaveInterval = 60;
        private SaveData _currentSaveData;
        public override void Init()
        {
            base.Init();
            SetupSaveData();
            PlayerController = FindObjectOfType<PlayerController>();
        }

        private void OnDisable()
        {
            SaveLoad.Save(_currentSaveData);
        }

        public List<ItemData> GetObtainedItems()
        {
            var list = new List<ItemData>();
            foreach (var item in _currentSaveData.obtainedItems)
            {
                var itemData = GlobalVariables.GetItemData(item);
                if (itemData != null)
                {
                    list.Add(itemData);
                }
            }

            return list;
        }

        private void SetupSaveData()
        {
            SaveLoad.Load();
            _currentSaveData = SaveLoad.CurrentSaveData;
            OnCurrencyChanged += _currentSaveData.UpdateCurrency;
            OnCropValueChanged += _currentSaveData.UpdateCropValue;
            OnItemObtained += _currentSaveData.ObtainItem;
            OnItemRemoved += _currentSaveData.RemoveItem;
            OnItemEquipped += _currentSaveData.EquipItem;
            OnItemUnequipped += _currentSaveData.UnequipItem;
            StartCoroutine(AutoSave());
        }

        private IEnumerator AutoSave()
        {
            var interval = new WaitForSeconds(autoSaveInterval);
            while (autoSave)
            {
                yield return interval;
                SaveLoad.Save(_currentSaveData);
            }
        }

        public static void ObtainCrop(int value)
        {
            var finalValue = CropValue + value;
            OnCropValueChanged?.Invoke(finalValue);
        }

        public static void AddCurrency(int amount)
        {
            var total = CurrentCurrency + amount;
            OnCurrencyChanged?.Invoke(total);
        }

        public static void SpendCurrency(int amount)
        {
            var total = CurrentCurrency - amount;
            OnCurrencyChanged?.Invoke(total);
        }

        public static void SellAllCrops()
        {
            AddCurrency(CropValue);
            OnCropValueChanged?.Invoke(0);
        }

        public static bool TryBuyItem(ItemData itemData)
        {
            if (itemData == null) return false;
            if (CurrentCurrency < itemData.itemCost) return false;

            SpendCurrency(itemData.itemCost);
            OnItemObtained?.Invoke(itemData.itemId);
            return true;

        }

        public static void Equip(ItemData itemData)
        {
            OnItemEquipped?.Invoke(itemData);
            Instance.PlayerController.Equip(itemData);
        }

        public static void Unequip(ItemType itemType)
        {
            OnItemUnequipped?.Invoke(itemType);
            Instance.PlayerController.Unequip(itemType);
        }

        public static string GetEquippedItem(ItemType itemType)
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
                case ItemType.Crop:
                default:
                    return string.Empty;
            }
        }
    }
}