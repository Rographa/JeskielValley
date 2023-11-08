using Config;
using Items;

namespace Utilities
{
    public class GlobalVariables : MonoSingleton<GlobalVariables>
    {
        public DefaultAnimationKeyframes defaultAnimationKeyframes;

        public static ItemAnimationData GetDefaultItemAnimationData(string itemId, Enums.AnimationStates animationState) =>
            Instance.defaultAnimationKeyframes.GetDefaultItemAnimationData(itemId, animationState);
    }
}