using System;
using System.Collections.Generic;

namespace PlayerSaveData
{
    [Serializable]
    public class SaveData
    {
        public int currentCurrency;
        public List<string> obtainedItems;

        public SaveData()
        {
            currentCurrency = 0;
            obtainedItems = new();
        }

        public void UpdateCurrency(int amount)
        {
            currentCurrency = amount;
        }

        public void ObtainItem(string itemId)
        {
            obtainedItems.Add(itemId);
        }

        public void RemoveItem(string itemId)
        {
            if (!obtainedItems.Contains(itemId)) return;
            obtainedItems.Remove(itemId);
        }
    }
}