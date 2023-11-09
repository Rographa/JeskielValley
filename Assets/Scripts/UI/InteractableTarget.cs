using Gameplay;
using Interfaces;
using Managers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI
{
    public class InteractableTarget : MonoSingleton<InteractableTarget>
    {
        private static string InputText => InputManager.GetMainKeyName(PlayerAction.Interact);
        private static bool IsEnabled => CurrentTarget != null;
        public static IInteractable CurrentTarget;
        
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI interactText;

        
        public static void SetTarget(IInteractable interactable)
        {
            CurrentTarget = interactable;
            if (IsEnabled)
            {
                Instance.SetPosition(interactable.GetPosition());
                Instance.SetText(interactable.GetInteractionText());
            }

            Instance.CheckActive();
        }

        private void SetText(string interactionText)
        {
            var finalText = $"[{InputText}] - {interactionText}";
            interactText.SetText(finalText);
            
        }

        private void CheckActive()
        {
            interactText.enabled = IsEnabled;
            image.enabled = IsEnabled;
        }

        private void SetPosition(Vector2 position)
        {
            transform.position = position;
        }

        public static void TryDisable(IInteractable interactable)
        {
            if (CurrentTarget == interactable)
            {
                SetTarget(null);
            }
        }
    }
}
