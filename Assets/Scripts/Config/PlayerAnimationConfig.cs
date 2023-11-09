using System;
using System.Collections.Generic;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "Config/Player Animation Config", fileName = "New Player Animation Config")]
    public class PlayerAnimationConfig : ScriptableObject
    {
        public List<AnimationData> playerAnimationData = new();

        public AnimationData GetStateData(AnimationStates state)
        {
            return playerAnimationData.Find(p => p.animationState == state);
        }
        
        public int GetIndex(AnimationStates state, Sprite currentSprite, Vector2 currentDirection)
        {
            var stateData = GetStateData(state);
            var sprites = stateData.GetSpritesByDirection(currentDirection);
            return sprites.IndexOf(currentSprite);
        }

        
    }

    [Serializable]
    public class AnimationData
    {
        public AnimationStates animationState;
        public List<Sprite> downSprites = new();
        public List<Sprite> upSprites = new();
        public List<Sprite> rightSprites = new();
        public List<Sprite> leftSprites = new();
        
        public List<Sprite> GetSpritesByDirection(Vector2 direction)
        {
            if (direction == Vector2.zero || direction.y <= -1) return downSprites;

            return direction.x switch
            {
                >= 1 => rightSprites,
                <= -1 => leftSprites,
                _ => upSprites
            };
        }

        public Sprite GetSprite(int index, Vector2 direction)
        {
            return GetSpritesByDirection(direction)[index];
        }
    }
}