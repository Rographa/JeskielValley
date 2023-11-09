using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private RectTransform inventoryPanel;
        [SerializeField] private RectTransform shopkeeperPanel;
        [SerializeField] private TextMeshProUGUI coinText;
        [SerializeField] private TextMeshProUGUI inventoryInputGuideText;
        [SerializeField] private float slideAnimationDuration = 0.6f;
        [SerializeField] private Ease slideAnimationEasing;
        
        private bool _isInventoryActive;
        private bool _isInventorySliding;
        private Tween _inventorySlideTween;
        
        private bool _isShopkeeperActive;
        private bool _isShopkeeperSliding;
        private Tween _shopkeeperSlideTween;

        public override void Init()
        {
            base.Init();
            var text = $"[{InputManager.GetMainKeyName(PlayerAction.Inventory)}] Inventory";
            inventoryInputGuideText.SetText(text);
            UpdateCoinText(GameManager.CurrentCurrency);
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
            GameManager.OnCurrencyChanged += UpdateCoinText;
        }

        private void Unsubscribe()
        {
            GameManager.OnCurrencyChanged -= UpdateCoinText;
        }
        
        private void UpdateCoinText(int value)
        {
            coinText.SetText(value.ToString());
        }

        public void ToggleShopkeeper()
        {
            if (_isShopkeeperSliding) return;
            _isShopkeeperActive = !_isShopkeeperActive;
            SlideShopkeeperPanel();
        }

        public void DisableShopkeeper()
        {
            if (!_isShopkeeperActive) return;

            if (_isShopkeeperSliding && _shopkeeperSlideTween != null)
            {
                _shopkeeperSlideTween.Complete(true);
            }
            ToggleShopkeeper();
        }

        public void ToggleInventory()
        {
            if (_isInventorySliding) return;
            _isInventoryActive = !_isInventoryActive;
            SlideInventoryPanel();
        }

        private void SlideShopkeeperPanel()
        {
            if (_isShopkeeperSliding) return;
            _isShopkeeperSliding = true;
            var target = Mathf.Abs(shopkeeperPanel.anchoredPosition.x) * (_isShopkeeperActive ? -1 : 1);
            _shopkeeperSlideTween = shopkeeperPanel.DOAnchorPosX(target, slideAnimationDuration).SetEase(slideAnimationEasing);
            _shopkeeperSlideTween.OnComplete(() => _isShopkeeperSliding = false);
        }
        
        private void SlideInventoryPanel()
        {
            if (_isInventorySliding) return;
            _isInventorySliding = true;
            var target = Mathf.Abs(inventoryPanel.anchoredPosition.x) * (_isInventoryActive ? 1 : -1);
            _inventorySlideTween = inventoryPanel.DOAnchorPosX(target, slideAnimationDuration).SetEase(slideAnimationEasing);
            _inventorySlideTween.OnComplete(() => _isInventorySliding = false);
        }
    }
}