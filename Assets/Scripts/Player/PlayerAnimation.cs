using System;
using UnityEngine;

namespace Player
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator anim;

        private static int DirX => Animator.StringToHash("DirX");
        private static int DirY => Animator.StringToHash("DirY");
        
        private int CurrentSpeed => Animator.StringToHash("CurrentSpeed");

        private PlayerController _playerController;
        public void Init(PlayerController controller)
        {
            _playerController = controller;
            Subscribe();
        }

        private void LateUpdate()
        {
            UpdateAnimations();
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
