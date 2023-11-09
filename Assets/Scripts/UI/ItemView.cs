using System;
using Items;
using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class ItemView : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private static Sprite NoneSprite
        {
            get
            {
                if (_noneSprite != null) return _noneSprite;
                _noneSprite = Resources.Load<Sprite>(GlobalVariables.NoneItemIconEndpoint);
                return _noneSprite;
            }
        }
        private static Sprite _noneSprite;

        private static Sprite LockedSprite
        {
            get
            {
                if (_lockedSprite != null) return _lockedSprite;
                _lockedSprite = Resources.Load<Sprite>(GlobalVariables.LockedItemIconEndpoint);
                return _lockedSprite;
            }
        }

        private static Sprite _lockedSprite;

        public Action<ItemData> OnSelected;
        public bool isSelected;
        public bool isHovered;
        public bool isLocked;
        
        [SerializeField] private PointerImageInteractionData pointerImageInteractionData;
        [SerializeField] private Image itemIconImage;

        private bool _isNone;
        private ItemData _itemData;
        private string _itemId;

        public void Disable()
        {
            gameObject.SetActive(false);
        }

        public void SetItem(string itemId)
        {
            _itemId = itemId;
            SetItemData();
        }

        public void SetItem(ItemData item)
        {
            _itemId = item.itemId;
            SetItemData(item);
        }

        public void SetNone()
        {
            _isNone = true;
            UpdateVisuals();
        }

        private void SetItemData(ItemData item)
        {
            _itemData = item;
            
            UpdateVisuals();
        }

        private void SetItemData()
        {
            _itemData = Resources.Load<ItemData>(GlobalVariables.ItemDataResourcesEndpoint);
            UpdateVisuals();
        }

        private void CheckLock()
        {
            isLocked = !InventoryManager.HasItem(_itemId);

            if (isLocked)
            {
                pointerImageInteractionData.SetLocked();
            }
        }

        private void UpdateVisuals()
        {
            CheckLock();
            Sprite icon = null;
            if (_isNone)
            {
                icon = NoneSprite;
            }
            else
            {
                if (_itemData != null)
                {
                    icon = isLocked ? LockedSprite : _itemData.itemVisuals.icon;
                }
            }

            itemIconImage.enabled = icon != null;
            if (icon != null)
            {
                itemIconImage.sprite = icon;
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (isLocked) return;
            OnSelected?.Invoke(_itemData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (isLocked) return;
            isHovered = true;
            if (!isSelected)
            {
                pointerImageInteractionData.SetHover();
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (isLocked) return;
            isHovered = false;
            if (!isSelected)
            {
                pointerImageInteractionData.SetNormal();
            }
        }

        public void SetSelected(bool value)
        {
            if (isSelected == value) return;
            
            isSelected = value;
            switch (value)
            {
                case true:
                    pointerImageInteractionData.SetSelected();
                    break;
                case false:
                    if (isHovered) pointerImageInteractionData.SetHover();
                    else pointerImageInteractionData.SetNormal(); 
                    break;
            }
        }

        public bool IsItem(ItemData itemData)
        {
            if (itemData == null)
            {
                return _isNone;
            }
            
            return itemData.itemId == _itemId;
        }
    }

    [Serializable]
    public class PointerImageInteractionData
    {
        public Image targetImage;
        public Sprite normalSprite;
        public Sprite highlightSprite;
        public Sprite selectedSprite;
        public Sprite lockedSprite;

        public void SetHover()
        {
            targetImage.sprite = highlightSprite;
        }

        public void SetNormal()
        {
            targetImage.sprite = normalSprite;
        }

        public void SetSelected()
        {
            targetImage.sprite = selectedSprite;
        }

        public void SetLocked()
        {
            targetImage.sprite = lockedSprite;
        }
    }
}
