using UnityEngine;
using HBDinosaur_ER_Project.ItemData;

namespace HBDinosaur_ER_Project.StorageSystem
{
    public class StorageSlot 
    {
        public int X;
        public int Y;
        public int ItemId = -1;
        public int Amount;

        public bool IsEmpty => ItemId < 0 || Amount <= 0;

        public void Clear()
        {
            ItemId = -1;
            Amount = 0;
        }

        public void SetItem(int itemId, int amount)
        {
            if (itemId < 0 || amount <= 0)
            {
                Clear();
                return;
            }

            ItemId = itemId;
            Amount = amount;
        }
    }
}

