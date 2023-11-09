using System;
using Interfaces;
using Managers;
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

            cropSpriteRenderer.sprite = null;
            _currentCropData = null;
            _currentStage = null;
            _currentStageIndex = 0;
            _progress = 0;
            _isActive = false;
            _isReady = false;
        }

        public void OnInteractionFocusEnter()
        {
            Debug.Log($"Interaction Focus Entered: {gameObject.name}");
        }

        public void OnInteractionFocusExit()
        {
            Debug.Log($"Interaction Focus Exited: {gameObject.name}");
        }

        public Vector2 GetPosition()
        {
            return transform.position;
        }
    }
}
