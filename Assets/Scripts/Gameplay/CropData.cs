using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
    [CreateAssetMenu(menuName = "Game Design/New Crop", fileName = "NewCrop")]
    public class CropData : ScriptableObject
    {
        public CropType cropType;
        public List<CropStage> stages;
        public Sprite finalStageSprite;
    }

    [Serializable]
    public class CropStage
    {
        public int ticksForNext;
        public Sprite cropSprite;
    }
}