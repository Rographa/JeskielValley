using System;
using System.Collections.Generic;
using Items;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ShopkeeperPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI buyButtonText;
        [SerializeField] private TextMeshProUGUI sellButtonText;
        
        [SerializeField] private Button buyButton;
        [SerializeField] private Button sellButton;
        [SerializeField] private List<ItemTypeSection> itemTypeSectionList = new();

        private ItemView _lastSelectedItemView;

        private void Start()
        {
            SetupItemTypeSections();
            UpdateCropValueText(GameManager.CropValue);
        }

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
            ItemTypeSection.OnItemViewSelected += UpdateItemViewSelected;
            GameManager.OnCropValueChanged += UpdateCropValueText;
            GameManager.OnCurrencyChanged += (amount) => CheckButtons();
            buyButton.onClick.AddListener(Buy);
            sellButton.onClick.AddListener(SellCrops);
        }

        private void CheckButtons()
        {
            buyButton.interactable = CanBuy();
        }

        private void UpdateCropValueText(int amount)
        {
            var text = $"Sell Crops ({amount.ToString()})";
            sellButtonText.SetText(text);
            sellButton.interactable = CanSell();
        }

        private void Unsubscribe()
        {
            ItemTypeSection.OnItemViewSelected -= UpdateItemViewSelected;
            GameManager.OnCropValueChanged -= UpdateCropValueText;
            GameManager.OnCurrencyChanged -= (amount) => CheckButtons();
            buyButton.onClick.RemoveAllListeners();
            sellButton.onClick.RemoveAllListeners();
        }

        private void UpdateItemViewSelected(ItemView itemView, ItemData itemData)
        {
            if (_lastSelectedItemView == itemView) return;

            foreach (var itemTypeSection in itemTypeSectionList) 
            {
                itemTypeSection.DeselectAll();
            }
            _lastSelectedItemView = itemView;
            _lastSelectedItemView.SetSelected(true);

            UpdateBuyButton(itemData.itemCost);
        }

        private void UpdateBuyButton(int itemCost)
        {
            var text = $"Buy ({itemCost.ToString()})";
            buyButtonText.SetText(text);
            buyButton.interactable = CanBuy();
        }

        private void SellCrops()
        {
            GameManager.SellAllCrops();
            UpdateCropValueText(0);
        }

        private void Buy()
        {
            if (_lastSelectedItemView == null) return;

            if (GameManager.TryBuyItem(_lastSelectedItemView.ItemData))
            {
                _lastSelectedItemView.SetObtained(true);
                _lastSelectedItemView = null;
                CheckButtons();
                itemTypeSectionList.ForEach(i => i.CheckObtainedItems());
            }
        }

        private void SetupItemTypeSections()
        {
            foreach (var itemTypeSection in itemTypeSectionList)
            {
                itemTypeSection.Init();
            }
        }

        private bool CanBuy()
        {
            if (_lastSelectedItemView == null || InventoryManager.HasItem(_lastSelectedItemView.itemId)) return false;
            
            return GameManager.CurrentCurrency > _lastSelectedItemView.ItemData.itemCost;
        }

        private bool CanSell()
        {
            return GameManager.CropValue > 0;
        }
    }
}
