using System;
using Config;
using Gameplay;
using Items;
using UnityEngine;

namespace Utilities
{
    public class GlobalVariables : MonoSingleton<GlobalVariables>
    {
        public const string CropDataResourcesEndpoint = "ScriptableObjects/Crops/";
        public const string ItemDataResourcesEndpoint = "ScriptableObjects/Items/";
        public const string ItemIconsEndpoint = "ItemIcons/";
        public const string NoneItemIconEndpoint = "ItemIcons/None-icon";
        public const string LockedItemIconEndpoint = "ItemIcons/Locked-icon";
        public DefaultAnimationKeyframes defaultAnimationKeyframes;
        [SerializeField] private GenericCollectable genericCollectablePrefab;

        public static ItemAnimationData GetDefaultItemAnimationData(string itemId, AnimationStates animationState) =>
            Instance.defaultAnimationKeyframes.GetDefaultItemAnimationData(itemId, animationState);

        public static GenericCollectable SpawnGenericCollectable(Vector2 position, Action<ItemData> onCollect, ItemData itemData)
        {
            var prefab = Instance.InstantiateGenericCollectable(position);
            prefab.Setup(onCollect, itemData);
            return prefab;
        }

        public static ItemData GetItemDataFromCropData(CropData cropData, int quality)
        {
            var endpoint = ItemDataResourcesEndpoint + Enum.GetName(typeof(CropType), cropData.cropType);
            var clone = Instantiate(Resources.Load<ItemData>(endpoint));
            clone.SetQuality(quality);
            return clone;
        }

        private GenericCollectable InstantiateGenericCollectable(Vector2 position)
        {
            var prefab = Instantiate(genericCollectablePrefab, position, Quaternion.identity);
            return prefab;
        }
        
        
    }
}