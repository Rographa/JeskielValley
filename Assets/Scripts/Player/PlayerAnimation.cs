using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Config;
using UnityEngine;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        public event Action<AnimationStates> OnAnimationStateChanged;
        public event Action<AnimationStates, int, Vector2> OnSpriteChanged;

        [SerializeField] private Animator anim;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private PlayerAnimationConfig playerAnimationConfig;

        private List<EquipmentComponent> _equipmentComponents = new();
        private List<string> animationStateNames = new();

        private string _currentStateName;
        private Sprite _lastSprite;

        private AnimationStates CurrentState => Enum.Parse<AnimationStates>(_currentStateName);
        private Vector2 CurrentDirection => new Vector2(anim.GetFloat(DirX), anim.GetFloat(DirY));
        private static int DirX => Animator.StringToHash("DirX");
        private static int DirY => Animator.StringToHash("DirY");

        private int CurrentSpeed => Animator.StringToHash("CurrentSpeed");

        private PlayerController _playerController;

        public void Init(PlayerController controller)
        {
            _playerController = controller;
            Subscribe();
            SetupEquipmentComponents();
            StartCoroutine(WaitForSpriteChanges());
        }

        public EquipmentComponent GetEquipmentComponent(ItemType itemType)
        {
            return _equipmentComponents.Find(i => i.itemType == itemType);
        }

        private void SetupEquipmentComponents()
        {
            _equipmentComponents ??= new();
            if (_equipmentComponents.Count > 0) return;

            var components = GetComponentsInChildren<EquipmentComponent>();
            _equipmentComponents.AddRange(components);

            foreach (var equipmentComponent in _equipmentComponents)
            {
                equipmentComponent.Init(this);
            }
        }

        private void LateUpdate()
        {
            UpdateAnimations();
            CheckAnimationState();
            
        }

        private void CheckSpriteChanges()
        {
            _lastSprite = spriteRenderer.sprite;
            var index = playerAnimationConfig.GetIndex(CurrentState, _lastSprite, CurrentDirection);
            UpdateEquipmentAnimations(index);
            OnSpriteChanged?.Invoke(CurrentState, index, CurrentDirection);
        }

        private void UpdateEquipmentAnimations(int index)
        {
            foreach (var equipmentComponent in _equipmentComponents)
            {
                equipmentComponent.UpdateInfo(CurrentState, index, CurrentDirection);
            }
        }

        private IEnumerator WaitForSpriteChanges()
        {
            var endOfFrame = new WaitForEndOfFrame();
            yield return endOfFrame;
            while (true)
            {
                yield return new WaitUntil(() => _lastSprite != spriteRenderer.sprite);
                CheckSpriteChanges();
                yield return endOfFrame;
            }
        }

        private void CheckAnimationState()
        {
            if (animationStateNames == null || animationStateNames.Count == 0)
            {
                SetupAnimationStateNames();
            }

            if (!HasSwitchedStates()) return;
            
            var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            foreach (var stateName in animationStateNames)
            {
                if (_currentStateName == stateName) continue;
                if (stateInfo.IsName(stateName))
                {
                    _currentStateName = stateName;
                    break;
                }
            }
            OnAnimationStateChanged?.Invoke(CurrentState);
        }

        private bool HasSwitchedStates()
        {
            return !anim.GetCurrentAnimatorStateInfo(0).IsName(_currentStateName);
        }

        private void SetupAnimationStateNames()
        {
            animationStateNames ??= new();
            if (animationStateNames.Count > 0) return;
            foreach (var stateName in Enum.GetNames(typeof(AnimationStates)))
            {
                if (stateName == "None") continue;
                animationStateNames.Add(stateName);
            }

            _currentStateName = animationStateNames.FirstOrDefault()?.ToString();
        }

        private void UpdateAnimations()
        {
            Set(CurrentSpeed, _playerController.CurrentSpeed);
        }

        private void Subscribe()
        {
            _playerController.OnDirectionChanged += UpdateAnimationDirection;
        }
        private void Unsubscribe()
        {
            _playerController.OnDirectionChanged -= UpdateAnimationDirection;
        }
        
        private void UpdateAnimationDirection(Vector2 dir)
        {
            Set(DirX, dir.x);
            Set(DirY, dir.y);
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        public void Set(string parameterName, object parameterValue)
        {
            switch (parameterValue)
            {
                case bool boolValue:
                    anim.SetBool(parameterName, boolValue);
                    break;
                case float floatValue:
                    anim.SetFloat(parameterName, floatValue);
                    break;
                case int intValue:
                    anim.SetInteger(parameterName, intValue);
                    break;
                default:
                    anim.SetTrigger(parameterName);
                    break;
            }
        }
        
        public void Set(int parameterHash, object parameterValue)
        {
            switch (parameterValue)
            {
                case bool boolValue:
                    anim.SetBool(parameterHash, boolValue);
                    break;
                case float floatValue:
                    anim.SetFloat(parameterHash, floatValue);
                    break;
                case int intValue:
                    anim.SetInteger(parameterHash, intValue);
                    break;
                default:
                    anim.SetTrigger(parameterHash);
                    break;
            }
        }

        private void SetEquipmentComponentActive(List<ItemType> list, bool value)
        {
            list.ForEach(i => GetEquipmentComponent(i).SetActive(value));
        }

        public void HideEquipments(List<ItemType> itemsToHide)
        {
            SetEquipmentComponentActive(itemsToHide, false);
        }

        public void ShowEquipments(List<ItemType> itemsToShow)
        {
            SetEquipmentComponentActive(itemsToShow, true);
        }
    }
}
