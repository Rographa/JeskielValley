using System;
using System.Collections.Generic;
using System.Linq;
using Config;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Items
{
    [CreateAssetMenu(menuName = "Game Design/New Item", fileName = "New Item")]
    public class ItemData : ScriptableObject
    {
        public string itemId;
        public Enums.ItemType itemType;
        public ItemVisuals itemVisuals;
        public int itemCost;

        private bool _isVisualsLoaded;

        public Sprite GetCurrentSprite(Enums.AnimationStates animationState, int index, Vector2 direction)
        {
            if (!_isVisualsLoaded)
            {
                LoadVisuals();
            }

            return itemVisuals.GetSprite(animationState, index, direction);
        }

        public void LoadVisuals()
        {
            if (_isVisualsLoaded) return;
            itemVisuals.Init(itemId);
            _isVisualsLoaded = true;
        }
    }

    [Serializable]
    public class ItemVisuals
    {
        public Sprite icon;
        public List<ItemAnimationData> animationData = new();

        public void Init(string itemId)
        {
            animationData ??= new();
            foreach (Enums.AnimationStates state in Enum.GetValues(typeof(Enums.AnimationStates)))
            {
                if (state == Enums.AnimationStates.None) continue;
                
                animationData.Add(GlobalVariables.GetDefaultItemAnimationData(itemId, state));
            }
        }

        public ItemAnimationData GetItemAnimationData(Enums.AnimationStates animationState)
        {
            return animationData.Find(i => i.animationState == animationState);
        }

        public Sprite GetSprite(Enums.AnimationStates animationState, int index, Vector2 direction) => GetItemAnimationData(animationState).GetSprite(index, direction);

    }

    [Serializable]
    public class ItemAnimationData
    {
        public Enums.AnimationStates animationState;
        public List<int> downKeyframes = new();
        public List<int> upKeyframes = new();
        public List<int> rightKeyframes = new();
        public List<int> leftKeyframes = new();

        [HideInInspector] public List<Sprite> allSprites;
        
        public ItemAnimationData(string itemId)
        {
            allSprites = new (Resources.LoadAll<Sprite>($"Sprites/{itemId}"));
        }
        
        public List<int> GetKeyframeListByDirection(Vector2 direction)
        {
            if (direction.y <= -1) return downKeyframes;

            return direction.x switch
            {
                >= 1 => rightKeyframes,
                <= -1 => leftKeyframes,
                _ => upKeyframes
            };
        }

        public Sprite GetSprite(int index, Vector2 direction)
        {
            var list = GetKeyframeListByDirection(direction);
            if (index >= list.Count)
            {
                index = Mathf.Clamp(index % list.Count, 0, list.Count - 1);
            }
            return allSprites[list[index]];
        }

        public List<Sprite> GetSpriteList(Vector2 direction)
        {
            var keyframeList = GetKeyframeListByDirection(direction);
            var list = allSprites.Where(sprite => keyframeList.Contains(allSprites.IndexOf(sprite))).ToList();
            return list;
        }
    }
}
