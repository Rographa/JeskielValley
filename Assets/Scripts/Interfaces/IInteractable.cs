using UnityEngine;

namespace Interfaces
{
    public interface IInteractable
    {
        public void Interact();
        public void OnInteractionFocusEnter();
        public void OnInteractionFocusExit();

        public Vector2 GetPosition();
    }
}