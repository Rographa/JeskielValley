using System;
using System.Collections.Generic;
using Managers;
using UnityEngine;
using Utilities;

namespace UI
{
    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private List<ItemTypeSection> itemTypeSectionList = new();

        private void Start()
        {
            SetupItemTypeSections();
        }

        private void OnEnable()
        {
            Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            GameManager.OnItemObtained += OnItemObtained;
        }

        private void Unsubscribe()
        {
            GameManager.OnItemObtained -= OnItemObtained;
        }
        
        private void OnItemObtained(string itemId)
        {
            itemTypeSectionList.ForEach(i => i.UnlockItem(itemId));
        }

        private void SetupItemTypeSections()
        {
            foreach (var itemTypeSection in itemTypeSectionList)
            {
                itemTypeSection.Init();
            }
        }
    }
}
