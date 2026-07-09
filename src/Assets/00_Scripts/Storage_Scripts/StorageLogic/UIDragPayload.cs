using UnityEngine;

namespace HBDinosaur_ER_Project.StorageSystem
{
    public enum ItemContainerType
    {
        Inventory,
        Storage,
        Loot,
        Equipment
    }

    public struct UIDragPayload
    {
        public ItemContainerType ContainerType;
        public int X;
        public int Y;
        public int ItemId;
        public int Amount;

        public UIDragPayload(ItemContainerType containerType, int x, int y, int itemId, int amount)
        {
            ContainerType = containerType;
            X = x;
            Y = y;
            ItemId = itemId;
            Amount = amount;
        }
    }
}

