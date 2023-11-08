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
        public event Action<Enums.AnimationStates> OnAnimationStateChanged;
        public event Action<int, Vector2> OnSpriteChanged;
        
        [SerializeField] private Animator anim;
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private PlayerAnimationConfig playerAnimationConfig;

        private List<EquipmentComponent> _equipmentComponents = new();
        private List<string> animationStateNames = new();

        private string _currentStateName;
        private Sprite _lastSprite;

        private Enums.AnimationStates CurrentState => Enum.Parse<Enums.AnimationStates>(_currentStateName);
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
            StartCoroutine(CheckAnimationState());
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
            CheckSpriteChanges();
        }

        private void CheckSpriteChanges()
        {
            if (_lastSprite == spriteRenderer.sprite) return;

            _lastSprite = spriteRenderer.sprite;
            var index = playerAnimationConfig.GetIndex(CurrentState, _lastSprite, CurrentDirection);
            OnSpriteChanged?.Invoke(index, CurrentDirection);
        }

        private IEnumerator CheckAnimationState()
        {
            SetupAnimationStateNames();
            _currentStateName = animationStateNames.FirstOrDefault().ToString();
            while (true)
            {
                yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsName(_currentStateName));
                var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                foreach (var stateName in animationStateNames)
                {
                    if (_currentStateName == stateName) continue;
                    if (stateInfo.IsName(stateName))
                    {
                        _currentStateName = stateName;
                    }
                }
                
                Debug.Log($"New animation state.{_currentStateName}");
                OnAnimationStateChanged?.Invoke(CurrentState);
                yield return new WaitForEndOfFrame();
            }
        }

        private void SetupAnimationStateNames()
        {
            animationStateNames ??= new();
            if (animationStateNames.Count > 0) return;
            foreach (var stateName in Enum.GetNames(typeof(Enums.AnimationStates)))
            {
                if (stateName == "None") continue;
                animationStateNames.Add(stateName);
            }
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
    }
}
