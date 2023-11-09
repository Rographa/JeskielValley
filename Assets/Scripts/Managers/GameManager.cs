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
        public static Action<string> OnItemObtained;
        public static Action<string> OnItemRemoved;

        public static int CropValue => Instance._currentSaveData.totalCropValue;
        public static int CurrentCurrency => Instance._currentSaveData.currentCurrency;

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
                var itemData = Resources.Load<ItemData>(GlobalVariables.ItemDataResourcesEndpoint + item);
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
            OnItemObtained += _currentSaveData.ObtainItem;
            OnItemRemoved += _currentSaveData.RemoveItem;
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
            Instance._currentSaveData.CollectCrop(value);
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
            Instance._currentSaveData.ResetCropValue();
        }

        public static void TryBuyItem(ItemData itemData)
        {
            if (itemData == null) return;
            
            if (CurrentCurrency >= itemData.itemCost)
            {
                SpendCurrency(itemData.itemCost);
                OnItemObtained?.Invoke(itemData.itemId);
            }
        }
    }
}