using DG.Tweening;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class UIManager : MonoSingleton<UIManager>
    {
        [SerializeField] private RectTransform inventoryPanel;
        [SerializeField] private float slideAnimationDuration = 0.6f;
        [SerializeField] private Ease slideAnimationEasing;
        
        private bool _isInventoryActive;
        private bool _isSliding;

        public void ToggleInventory()
        {
            if (_isSliding) return;
            _isInventoryActive = !_isInventoryActive;
            SlideInventoryPanel();
        }
        
        private void SlideInventoryPanel()
        {
            if (_isSliding) return;
            _isSliding = true;
            var target = Mathf.Abs(inventoryPanel.anchoredPosition.x) * (_isInventoryActive ? 1 : -1);
            var tween = inventoryPanel.DOAnchorPosX(target, slideAnimationDuration).SetEase(slideAnimationEasing);
            tween.OnComplete(() => _isSliding = false);
        }
    }
}