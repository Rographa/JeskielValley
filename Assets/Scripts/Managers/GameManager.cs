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

        [SerializeField] private bool autoSave = true;
        [SerializeField] private float autoSaveInterval = 60;
        private SaveData _currentSaveData;
        public override void Init()
        {
            base.Init();
            SetupSaveData();
            PlayerController = FindObjectOfType<PlayerController>();
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
    }
}