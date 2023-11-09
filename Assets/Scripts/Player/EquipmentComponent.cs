using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Items;
using Managers;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Vector2 = UnityEngine.Vector2;

namespace Player
{
    public class EquipmentComponent : MonoBehaviour
    {
        public ItemType itemType;
        [SerializeField] private List<ItemType> itemsToHideWhenEquipped = new();
        [SerializeField] private string preEquippedItemId;

        private SpriteRenderer SpriteRenderer
        {
            get
            {
                _spriteRenderer ??= GetComponent<SpriteRenderer>();
                return _spriteRenderer;
            }
        }
        private ItemData _currentItem;
        private Vector2 _currentDirection;
        private PlayerAnimation _playerAnimation;
        private AnimationStates _currentState;
        private int _spriteIndex;

        private bool _isInitialized;
        private bool _isEnabled;
        private SpriteRenderer _spriteRenderer;

        public ItemData CurrentItem => _currentItem;

        public void Init(PlayerAnimation playerAnimation)
        {
            _playerAnimation = playerAnimation;
            SetupInitialValues();
            preEquippedItemId = GameManager.GetEquippedItem(itemType);
            if (!string.IsNullOrEmpty(preEquippedItemId))
            {
                var preEquippedItem = GlobalVariables.GetItemData(preEquippedItemId);
                EquipItem(preEquippedItem);
            }
            
            SetActive(true);
            _isInitialized = true;
        }

        private void SetupInitialValues()
        {
            _currentDirection = Vector2.down;
            _currentState = AnimationStates.Idle;
        }
        private void SetCurrentSprite(Sprite sprite)
        {
            SpriteRenderer.sprite = sprite;
        }

        public void EquipItem(ItemData item)
        {
            if (item == null)
            {
                Unequip();
                return;
            }

            if (item.itemType != itemType) return;

            _currentItem = item;
            _currentItem.ResetVisuals();
            OnItemEquipped();
        }

        private void OnItemEquipped()
        {
            _playerAnimation.HideEquipments(itemsToHideWhenEquipped);
            UpdateVisuals();
        }

        private void OnItemUnequipped()
        {
            _playerAnimation.ShowEquipments(itemsToHideWhenEquipped);
        }

        private void Unequip()
        {
            _currentItem = null;
            SetCurrentSprite(null);
            OnItemUnequipped();
        }

        public void UpdateInfo(AnimationStates currentState, int index, Vector2 currentDirection)
        {
            if (!_isEnabled || _currentItem == null || !_isInitialized) return;
            
            _currentState = currentState;
            _spriteIndex = index == -1 ? _spriteIndex + 1 : index;
            _currentDirection = currentDirection;

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            SetCurrentSprite(_currentItem.GetCurrentSprite(_currentState, _spriteIndex, _currentDirection));
        }

        public void SetActive(bool value)
        {
            _isEnabled = value;
            SpriteRenderer.enabled = value;
        }
    }
}
