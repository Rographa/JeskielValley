using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using Interfaces;
using UnityEngine;

namespace Player
{
    public class PlayerCollision : MonoBehaviour
    {
        [SerializeField] private LayerMask collectableLayerMask;
        private IInteractable _closestInteractable;
        private float _magnetRange;
        private float _interactRange;
        
        private List<ICollectable> _collectablesInMagnetRange = new();
        public void Init(PlayerStatsConfig stats)
        {
            _magnetRange = stats.magnetRange;
            _interactRange = stats.interactionRange;
        }

        private void FixedUpdate()
        {
            HandleMagnet();
            HandleInteraction();
        }

        private Collider2D[] OverlapAll(float range, LayerMask mask)
        {
            return Physics2D.OverlapCircleAll(transform.position, range, mask);
        }

        private void HandleInteraction()
        {
            var colliders = OverlapAll(_interactRange, ~0);
            var interactables = colliders.Select(hit => hit.GetComponent<IInteractable>()).Where(interactable => interactable != null).ToList();

            if (interactables.Count == 0)
            {
                _closestInteractable?.OnInteractionFocusExit();
                _closestInteractable = null;
                return;
            }
            var closest = interactables.First();
            var nearestDistance = GetDistance(closest.GetPosition());
            foreach (var interactable in interactables)
            {
                if (interactable == closest) continue;
                var distance = GetDistance(interactable.GetPosition());

                if (distance >= nearestDistance) continue;
                
                closest = interactable;
                nearestDistance = distance;
            }

            if (_closestInteractable == closest) return;
            
            _closestInteractable?.OnInteractionFocusExit();
            _closestInteractable = closest;
            _closestInteractable?.OnInteractionFocusEnter();
        }

        private float GetDistance(Vector2 position)
        {
            return Vector2.Distance(transform.position, position);
        }

        private void HandleMagnet()
        {
            var colliders = OverlapAll(_magnetRange, collectableLayerMask);

            var collectables = new List<ICollectable>();
            foreach (var hit in colliders)
            {
                var collectable = hit.GetComponent<ICollectable>();
                if (collectable == null) continue;

                collectables.Add(collectable);
            }
            UpdateCollectablesInMagnetRange(collectables);
        }

        private void UpdateCollectablesInMagnetRange(List<ICollectable> collectables)
        {
            var enteredThisFrame = collectables.FindAll(c => !_collectablesInMagnetRange.Contains(c));
            var exitedThisFrame = _collectablesInMagnetRange.FindAll(c => !collectables.Contains(c));
            
            enteredThisFrame.ForEach(i => i.OnMagnetRangeEnter(transform));
            exitedThisFrame.ForEach(i => i.OnMagnetRangeExit(transform));

            _collectablesInMagnetRange = collectables;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            var collectable = other.GetComponent<ICollectable>();
            collectable?.Collect();
        }

        public IInteractable GetInteractable()
        {
            return _closestInteractable;
        }
    }
}