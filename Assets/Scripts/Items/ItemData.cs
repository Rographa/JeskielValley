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
        public ItemType itemType;
        public ItemVisuals itemVisuals;
        public int itemCost;

        private bool IsVisualsLoaded { get; set; }

        public Sprite GetCurrentSprite(AnimationStates animationState, int index, Vector2 direction)
        {
            if (!IsVisualsLoaded)
            {
                LoadVisuals();
            }

            return itemVisuals.GetSprite(animationState, index, direction);
        } 
        public void LoadVisuals()
        {
            if (IsVisualsLoaded) return;
            itemVisuals.Init(itemId, itemType);
            IsVisualsLoaded = true;
        }

        public void ResetVisuals()
        {
            IsVisualsLoaded = false;
            LoadVisuals();
        }

        public void SetQuality(int quality)
        {
            itemCost += (itemCost * quality / 100);
        }
    }

    [Serializable]
    public class ItemVisuals
    {
        public Sprite icon;
        public List<ItemAnimationData> animationData = new();

        private string itemId;

        public void Init(string itemId, ItemType itemType)
        {
            this.itemId = itemId;
            if (itemType == ItemType.Crop) return;
            
            LoadAnimationData();
            if (icon == null)
            {
                TryLoadIcon();
            }
        }

        private void TryLoadIcon()
        {
            var endpoint = GlobalVariables.ItemIconsEndpoint + $"{itemId}-icon";
            icon = Resources.Load<Sprite>(endpoint);
        }

        private void LoadAnimationData()
        {
            animationData = new();
            foreach (AnimationStates state in Enum.GetValues(typeof(AnimationStates)))
            {
                if (state == AnimationStates.None) continue;
                
                animationData.Add(GlobalVariables.GetDefaultItemAnimationData(itemId, state));
            }
        }

        public ItemAnimationData GetItemAnimationData(AnimationStates animationState)
        {
            return animationData.Find(i => i.animationState == animationState);
        }

        public Sprite GetSprite(AnimationStates animationState, int index, Vector2 direction) => GetItemAnimationData(animationState).GetSprite(index, direction);

    }

    [Serializable]
    public class ItemAnimationData
    {
        public AnimationStates animationState;
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
            if (direction == Vector2.zero || direction.y <= -1) return downKeyframes;

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
