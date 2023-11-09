using System.Collections.Generic;
using Items;
using UnityEngine;

namespace Config
{
    [CreateAssetMenu(menuName = "Config/New Default Animation Keyframes", fileName = "DefaultAnimationKeyframes")]
    public class DefaultAnimationKeyframes : ScriptableObject
    {
        public List<ItemAnimationData> defaultItemAnimationData;

        public ItemAnimationData GetDefaultItemAnimationData(string itemId, AnimationStates animationState)
        {
            var defaultItem = defaultItemAnimationData.Find(i => i.animationState == animationState);
            var clone = new ItemAnimationData(itemId)
            {
                animationState = animationState,
                downKeyframes = defaultItem.downKeyframes,
                upKeyframes = defaultItem.upKeyframes,
                rightKeyframes = defaultItem.rightKeyframes,
                leftKeyframes = defaultItem.leftKeyframes
            };
            return clone;
        }
    }
}