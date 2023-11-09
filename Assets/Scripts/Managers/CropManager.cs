using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Items;
using UnityEngine;
using Utilities;

namespace Managers
{
    public class CropManager : MonoSingleton<CropManager>
    {
        [SerializeField] private float tickIntervalInSeconds;
        [SerializeField] private Vector2Int qualityPerTickRange;
        [SerializeField] private List<CropPoint> cropPoints;

        public override void Init()
        {
            base.Init();
            StartCoroutine(HandleTick());
        }

        private IEnumerator HandleTick()
        {
            var interval = new WaitForSeconds(tickIntervalInSeconds);
            while (true)
            {
                yield return interval;
                Tick();
            }
        }

        private void Tick()
        {
            cropPoints.ForEach(crop => crop.OnTick());
        }

        public static int GetRandomQualityProgress()
        {
            var min = Instance.qualityPerTickRange.x;
            var max = Instance.qualityPerTickRange.y;
            return Random.Range(min, max);
        }

        public static void GenerateCollectableCrop(CropData currentCropData, int quality, Vector2 position)
        {
            var itemData = GlobalVariables.GetItemDataFromCropData(currentCropData, quality);
            var collectable = GlobalVariables.SpawnGenericCollectable(position, OnCollect, itemData);
        }

        private static void OnCollect(ItemData itemData)
        {
            InventoryManager.ObtainCrop(itemData);
        }
    }
}