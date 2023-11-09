using System;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class InventoryPanel : MonoBehaviour
    {
        [SerializeField] private List<ItemTypeSection> itemTypeSectionList = new();

        private void Start()
        {
            SetupItemTypeSections();
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
