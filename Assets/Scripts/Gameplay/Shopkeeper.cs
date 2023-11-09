using Interfaces;
using Managers;
using UI;
using UnityEngine;

namespace Gameplay
{
    public class Shopkeeper : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            UIManager.Instance.ToggleShopkeeper();
        }

        public void OnInteractionFocusEnter()
        {
            InteractableTarget.SetTarget(this);
        }

        public void OnInteractionFocusExit()
        {
            UIManager.Instance.DisableShopkeeper();
            InteractableTarget.TryDisable(this);
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }

        public string GetInteractionText()
        {
            return "Trade";
        }

        public bool CanInteract()
        {
            return true;
        }
    }
}
