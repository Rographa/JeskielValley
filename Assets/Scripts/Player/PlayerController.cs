using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using Items;
using Managers;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        public event Action<Vector2> OnDirectionChanged;
        
        [SerializeField] private PlayerStatsConfig baseStats;
        [SerializeField] private PlayerAnimation playerAnimation;
        [SerializeField] private PlayerCollision playerCollision;
        [SerializeField] private Rigidbody2D rb;

        private Dictionary<PlayerAction, bool> _pressedInputs = new();
        private bool _canWalk = true;
        private bool _isTired = false;
        private float _currentStamina;

        private Coroutine _staminaHandler;

        #region GETTERS
        public Vector2 LastDirection { get; private set; }

        public float CurrentSpeed => rb.velocity.magnitude;
        private bool UpPressed => _pressedInputs.Any(input => input is { Key: PlayerAction.MoveUp, Value: true });
        private bool DownPressed => _pressedInputs.Any(input => input is { Key: PlayerAction.MoveDown, Value: true });
        private bool RightPressed => _pressedInputs.Any(input => input is { Key: PlayerAction.MoveRight, Value: true });
        private bool LeftPressed => _pressedInputs.Any(input => input is { Key: PlayerAction.MoveLeft, Value: true });
        private bool SprintPressed => _pressedInputs.Any(input => input is { Key: PlayerAction.Sprint, Value: true });
        private bool IsSprinting => SprintPressed && !_isTired;

        private bool IsMoving => _canWalk && (UpPressed || DownPressed || RightPressed || LeftPressed);
        
        private bool IsMovingVertically => (UpPressed && !DownPressed) || (DownPressed && !UpPressed);
        
        private bool IsMovingHorizontally => (RightPressed && !LeftPressed) || (LeftPressed && !RightPressed);
        public ItemData EquippedHair => playerAnimation.GetEquipmentComponent(ItemType.Hair).CurrentItem;
        public ItemData EquippedOutfit => playerAnimation.GetEquipmentComponent(ItemType.Outfit).CurrentItem;
        public ItemData EquippedHat => playerAnimation.GetEquipmentComponent(ItemType.Hat).CurrentItem;

        #endregion
        
        private void Init()
        {
            SetupStats();
            playerAnimation.Init(this);
            playerCollision.Init(baseStats);
            StartCoroutine(MoveRoutine());
        }

        private void SetupStats()
        {
            baseStats = Instantiate(baseStats);
            _currentStamina = baseStats.maxStamina;
            _staminaHandler = StartCoroutine(HandleStamina());
        }
        
        private void OnEnable()
        {
            Init();
            Subscribe();
        }
        private void OnDisable()
        {
            Unsubscribe();
        }

        private IEnumerator HandleStamina()
        {
            var endOfFrame = new WaitForEndOfFrame();
            while (true)
            {
                while (!IsSprinting && !_isTired)
                {
                    RecoverStamina(baseStats.staminaRegenPerSecond);
                    yield return endOfFrame;
                }
                yield return new WaitUntil(() => !IsSprinting && !_isTired);
            }
        }

        private void RecoverStamina(float amount)
        {
            _currentStamina = Mathf.Clamp(_currentStamina + amount, 0, baseStats.maxStamina);
        }

        private IEnumerator MoveRoutine()
        {
            var fixedUpdate = new WaitForFixedUpdate();
            while (true)
            {
                while (_canWalk)
                {
                    if (!IsMoving)
                    {
                        yield return fixedUpdate;
                        continue;
                    }
                    
                    GetInputDirection();
                    var isMovingDiagonally = IsMovingHorizontally && IsMovingVertically;
                    var speedMultiplier = isMovingDiagonally ? baseStats.diagonalMovementMultiplier : 1f;

                    TrySprint(ref speedMultiplier);
                    Move(LastDirection, speedMultiplier);
                    yield return fixedUpdate;
                }

                yield return new WaitUntil(() => _canWalk);
            }
        }

        private void GetInputDirection()
        {
            var dir = Vector2.zero;

            if (DownPressed)
            {
                dir += Vector2.down;
            } else if (UpPressed)
            {
                dir += Vector2.up;
            }
            if (RightPressed)
            {
                dir += Vector2.right;
            } else if (LeftPressed)
            {
                dir += Vector2.left;
            }
            if (LastDirection == dir) return;
            
            LastDirection = dir;
            OnDirectionChanged?.Invoke(LastDirection);
        }

        private bool TrySprint(ref float speedMultiplier)
        {
            if (!IsSprinting) return false;
            ConsumeStamina(baseStats.sprintStaminaCostPerSecond * Time.fixedDeltaTime);
            speedMultiplier *= baseStats.sprintSpeedMultiplier;
            return true;
        }

        private void ConsumeStamina(float amount)
        {
            _currentStamina -= amount;
            if (_currentStamina > 0) return;
            
            _currentStamina = 0;
            GetTired();
        }

        private void GetTired()
        {
            _isTired = true;
            StartCoroutine(HandleTiredStatus());
        }

        private IEnumerator HandleTiredStatus()
        {
            yield return new WaitForSeconds(baseStats.tiredStatusDuration);
            _isTired = false;
        }

        private void Move(Vector2 direction, float multiplier = 1f)
        {
            rb.AddForce(direction * (baseStats.moveSpeed * multiplier * Time.fixedDeltaTime));
        }

        private void Subscribe()
        {
            InputManager.OnInputDown += ResolveInputDown;
            InputManager.OnInputUp += ResolveInputUp;
        }

        private void Unsubscribe()
        {
            InputManager.OnInputDown -= ResolveInputDown;
            InputManager.OnInputUp -= ResolveInputUp;
        }
        
        private void ResolveInputDown(PlayerAction input)
        {
            if (!_pressedInputs.TryAdd(input, true))
                _pressedInputs[input] = true;
        }
    
        private void ResolveInputUp(PlayerAction input)
        {
            if (!_pressedInputs.TryAdd(input, false))
                _pressedInputs[input] = false;

            switch (input)
            {
                case PlayerAction.Interact:
                    TryInteract();
                    break;
                case PlayerAction.None:
                case PlayerAction.MoveLeft:
                case PlayerAction.MoveRight:
                case PlayerAction.MoveUp:
                case PlayerAction.MoveDown:
                case PlayerAction.Sprint:
                case PlayerAction.Jump:
                case PlayerAction.Inventory:
                default:
                    break;
            }
        }

        private void TryInteract()
        {
            playerCollision.GetInteractable()?.Interact();
        }

        public void Equip(ItemData itemData)
        {
            var equipmentComponent = playerAnimation.GetEquipmentComponent(itemData.itemType);
            equipmentComponent.EquipItem(itemData);
        }

        public void Unequip(ItemType itemType)
        {
            var equipmentComponent = playerAnimation.GetEquipmentComponent(itemType);
            equipmentComponent.EquipItem(null);
        }
    }
}
