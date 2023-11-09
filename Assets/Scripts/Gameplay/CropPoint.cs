using Interfaces;
using Managers;
using UnityEngine;

namespace Gameplay
{
    public class CropPoint : MonoBehaviour, IInteractable
    {
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

            _currentStage = _currentCropData.stages[_currentStageIndex];
        }

        private void SetCropReady()
        {
            _isReady = true;
        }

        public void Plant(CropData cropData)
        {
            _currentCropData = Instantiate(cropData);
            _currentStageIndex = 0;
            _progress = 0;
            _currentStage = _currentCropData.stages[_currentStageIndex];
            _maxStage = _currentCropData.stages.Count - 1;
        }

        public void Interact()
        {
            if (_isReady)
            {
                Harvest();
            }
        }

        private void Harvest()
        {
            
        }

        public void OnRangeEnter()
        {
            
        }

        public void OnRangeExit()
        {
            
        }
    }
}
