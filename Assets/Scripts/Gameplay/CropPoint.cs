using System;
using Interfaces;
using Managers;
using UI;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Gameplay
{
    public class CropPoint : MonoBehaviour, IInteractable
    {
        [SerializeField] private SpriteRenderer cropSpriteRenderer;
        
        private CropData _currentCropData;
        private CropStage _currentStage;
        private int _currentStageIndex;
        private int _maxStage;
        private int _progress;
        private int _quality;
        private bool _isActive;
        private bool _isReady;
        private int Target => _currentStage.ticksForNext;
        
        public void OnTick()
        {
            if (!_isActive || _isReady) return;
            
            _progress++;
            if (_progress >= Target)
            {
                GrowToNextStage();
            }
        }

        private void GrowToNextStage()
        {
            _progress = 0;
            _currentStageIndex++;
            _quality += CropManager.GetRandomQualityProgress();

            if (_currentStageIndex >= _maxStage)
            {
                SetCropReady();
                return;
            }
            SetStage();
        }

        private void SetCropReady()
        {
            _isReady = true;
            cropSpriteRenderer.sprite = _currentCropData.finalStageSprite;
        }

        private void SetStage()
        {
            _currentStage = _currentCropData.stages[_currentStageIndex];
            cropSpriteRenderer.sprite = _currentStage.cropSprite;
        }

        public void Plant(CropType cropType)
        {
            ResetCropPoint();
            var endpoint = GlobalVariables.CropDataResourcesEndpoint + Enum.GetName(typeof(CropType), cropType);
            _currentCropData = Instantiate(Resources.Load<CropData>(endpoint));
            _currentStageIndex = 0;
            _progress = 0;
            _maxStage = _currentCropData.stages.Count - 1;
            SetStage();
            _isActive = true;
        }

        public void Interact()
        {
            InteractableTarget.SetTarget(null);
            if (_isReady)
            {
                Harvest();
                return;
            }
            if (!_isActive)
            {
                var newCrop = Random.value > 0.5f ? CropType.Potato : CropType.Tomato;
                Plant(newCrop);
            }
        }

        private void Harvest()
        {
            var pos = cropSpriteRenderer.transform.position;
            CropManager.GenerateCollectableCrop(_currentCropData, _quality, pos);

            ResetCropPoint();
        }

        private void ResetCropPoint()
        {
            cropSpriteRenderer.sprite = null;
            _currentCropData = null;
            _currentStage = null;
            _currentStageIndex = 0;
            _progress = 0;
            _quality = 0;
            _isActive = false;
            _isReady = false;
        }

        public void OnInteractionFocusEnter()
        {
            Debug.Log($"Interaction Focus Entered: {gameObject.name}");
            InteractableTarget.SetTarget(this);
        }

        public void OnInteractionFocusExit()
        {
            Debug.Log($"Interaction Focus Exited: {gameObject.name}");
            InteractableTarget.TryDisable(this);
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }

        public string GetInteractionText()
        {
            return _isReady ? "Harvest" : "Plant";
        }

        public bool CanInteract()
        {
            return _isReady || !_isActive;
        }
    }
}
