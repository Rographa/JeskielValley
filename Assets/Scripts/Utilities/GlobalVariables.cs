using Config;
using Items;

namespace Utilities
{
    public class GlobalVariables : MonoSingleton<GlobalVariables>
    {
        public const string ItemDataResourcesEndpoint = "ScriptableObjects/Items/";
        public const string ItemIconsEndpoint = "ItemIcons/";
        public const string NoneItemIconEndpoint = "ItemIcons/None-icon";
        public const string LockedItemIconEndpoint = "ItemIcons/Locked-icon";
        public DefaultAnimationKeyframes defaultAnimationKeyframes;

        public static ItemAnimationData GetDefaultItemAnimationData(string itemId, AnimationStates animationState) =>
            Instance.defaultAnimationKeyframes.GetDefaultItemAnimationData(itemId, animationState);
    }
}