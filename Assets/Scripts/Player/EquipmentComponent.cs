using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Items;
using UnityEngine;
using UnityEngine.Serialization;
using Vector2 = UnityEngine.Vector2;

namespace Player
{
    public class EquipmentComponent : MonoBehaviour
    {
        [SerializeField] private Enums.ItemType itemType;
        [SerializeField] private ItemData preEquippedItem;

        private SpriteRenderer _spriteRenderer;
        private ItemData _currentItem;
        private Vector2 _currentDirection;
        private PlayerAnimation _playerAnimation;
        private Enums.AnimationStates _currentState;
        private int _spriteIndex;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Init()
        {
            SetupInitialValues();
            EquipItem(preEquippedItem);
        }

        private void SetupInitialValues()
        {
            _currentDirection = Vector2.down;
            _currentState = Enums.AnimationStates.Idle;
        }
        private void SetCurrentSprite(Sprite sprite)
        {
            _spriteRenderer.sprite = sprite;
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
            _currentItem.LoadVisuals();
        }

        private void Unequip()
        {
            _currentItem = null;
            SetCurrentSprite(null);
        }

        public void UpdateInfo(Enums.AnimationStates currentState, int index, Vector2 currentDirection)
        {
            if (_currentItem == null) return;
            
            _currentState = currentState;
            _spriteIndex = index == -1 ? _spriteIndex + 1 : index;
            _currentDirection = currentDirection;

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            SetCurrentSprite(_currentItem.GetCurrentSprite(_currentState, _spriteIndex, _currentDirection));
        }
    }
}
