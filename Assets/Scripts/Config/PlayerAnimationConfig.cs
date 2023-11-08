using System;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "Config/Player Animation Config", fileName = "New Player Animation Config")]
    public class PlayerAnimationConfig : ScriptableObject
    {
        public List<AnimationData> playerAnimationData = new();

        public AnimationData GetStateData(Enums.AnimationStates state)
        {
            return playerAnimationData.Find(p => p.animationState == state);
        }
        
        public int GetIndex(Enums.AnimationStates state, Sprite currentSprite, Vector2 currentDirection)
        {
            var stateData = GetStateData(state);
            var sprites = stateData.GetSpritesByDirection(currentDirection);
            return sprites.FindIndex(s => s.name == currentSprite.name);
        }

        
    }

    [Serializable]
    public class AnimationData
    {
        public Enums.AnimationStates animationState;
        public List<Sprite> downSprites = new();
        public List<Sprite> upSprites = new();
        public List<Sprite> rightSprites = new();
        public List<Sprite> leftSprites = new();
        
        public List<Sprite> GetSpritesByDirection(Vector2 direction)
        {
            return direction.x switch
            {
                >= 1 => rightSprites,
                <= -1 => leftSprites,
                _ => direction.y >= 1 ? upSprites : downSprites
            };
        }

        public Sprite GetSprite(int index, Vector2 direction)
        {
            return GetSpritesByDirection(direction)[index];
        }
    }
}