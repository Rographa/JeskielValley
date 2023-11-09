using Interfaces;
using Managers;
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
            Debug.Log("Shopkeeper enabled.");
        }

        public void OnInteractionFocusExit()
        {
            Debug.Log("Shopkeeper disabled.");
            UIManager.Instance.DisableShopkeeper();
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }
    }
}
