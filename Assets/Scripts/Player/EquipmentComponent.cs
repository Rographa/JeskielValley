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
        private List<Sprite> _currentAnimationCycle;
        private ItemData _currentItem;
        private Vector2 _currentDirection;
        private PlayerAnimation _playerAnimation;
        private Enums.AnimationStates _currentState;
        private int _spriteIndex;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void Init(PlayerAnimation playerAnimation)
        {
            _playerAnimation = playerAnimation;
            SetupInitialValues();
            Subscribe();
            EquipItem(preEquippedItem);
        }

        private void SetupInitialValues()
        {
            _currentDirection = Vector2.down;
            _currentState = Enums.AnimationStates.Idle;
            _currentAnimationCycle = new();
        }

        private void Subscribe()
        {
            _playerAnimation.OnAnimationStateChanged += OnAnimationStateChanged;
            _playerAnimation.OnSpriteChanged += OnSpriteChanged;
        }

        private void OnAnimationStateChanged(Enums.AnimationStates state)
        {
            if (_currentItem == null) return;
            _currentState = state;
            UpdateAnimationCycle();
        }

        private void OnDirectionChanged()
        {
            if (_currentItem == null) return;
            _spriteIndex = 0;
            UpdateAnimationCycle();
        }

        private void UpdateAnimationCycle()
        {
            if (_currentItem == null) return;
            _currentAnimationCycle =
                _currentItem.itemVisuals.GetItemAnimationData(_currentState).GetSpriteList(_currentDirection);
        }

        private void OnSpriteChanged(int index, Vector2 direction)
        {
            if (_currentItem == null || index == -1) return;

            if (_currentDirection != direction)
            {
                _currentDirection = direction;
                OnDirectionChanged();
            }

            _spriteIndex = index;
            if (_currentAnimationCycle.Count <= _spriteIndex)
                UpdateAnimationCycle();
            SetCurrentSprite(_currentAnimationCycle[_spriteIndex]);
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
            LoadItemVisuals();
        }

        private void LoadItemVisuals()
        {
            _currentItem.LoadVisuals();
            UpdateAnimationCycle();
        }

        private void Unequip()
        {
            _currentItem = null;
            _currentAnimationCycle.Clear();
            SetCurrentSprite(null);
        }
    }
}
